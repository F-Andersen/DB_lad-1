#!/usr/bin/env python3
"""
CLI Tool for PostgreSQL Sharding - Lab 6
Supports:
- apply-sql: Apply SQL scripts to all 16 databases with two-phase commit
- benchmark: Run performance benchmarks
"""

import argparse
import json
import sys
import time
import uuid
import random
import threading
from concurrent.futures import ThreadPoolExecutor, as_completed
from datetime import datetime

try:
    import psycopg2
    from psycopg2 import sql
except ImportError:
    print("Error: psycopg2 not installed. Run: pip install psycopg2-binary")
    sys.exit(1)


def load_mapping(mapping_file):
    """Load shard mapping from JSON file"""
    with open(mapping_file, 'r') as f:
        return json.load(f)


def parse_connection_string(conn_str):
    """Parse connection string to dict"""
    params = {}
    for part in conn_str.split(';'):
        if '=' in part:
            key, value = part.split('=', 1)
            params[key.lower()] = value
    return params


def get_shard_key(plan_id):
    """Get shard key from UUID (last hex character)"""
    return plan_id.replace('-', '')[-1].lower()


def apply_sql(mapping_file, sql_file):
    """Apply SQL script to all 16 databases with two-phase commit"""
    print(f"\n{'='*60}")
    print("  CLI Tool: Apply SQL to All Shards")
    print(f"{'='*60}")
    
    mapping = load_mapping(mapping_file)
    
    with open(sql_file, 'r') as f:
        sql_content = f.read()
    
    print(f"\nMapping file: {mapping_file}")
    print(f"SQL file: {sql_file}")
    print(f"Databases: {len(mapping)}")
    
    connections = {}
    cursors = {}
    
    print("\n--- Phase 1: BEGIN transactions ---")
    
    # Open connections and begin transactions
    try:
        for shard_key, conn_str in mapping.items():
            params = parse_connection_string(conn_str)
            conn = psycopg2.connect(
                host=params.get('host', 'localhost'),
                port=int(params.get('port', 5432)),
                database=params.get('database'),
                user=params.get('username'),
                password=params.get('password')
            )
            conn.autocommit = False
            connections[shard_key] = conn
            cursors[shard_key] = conn.cursor()
            print(f"  [{shard_key}] Connected to {params.get('database')} - BEGIN")
    except Exception as e:
        print(f"\nError connecting: {e}")
        for conn in connections.values():
            conn.close()
        return False
    
    print("\n--- Phase 2: EXECUTE SQL ---")
    
    # Execute SQL on all databases
    success = True
    for shard_key, cursor in cursors.items():
        try:
            cursor.execute(sql_content)
            print(f"  [{shard_key}] SQL executed successfully")
        except Exception as e:
            print(f"  [{shard_key}] ERROR: {e}")
            success = False
            break
    
    print(f"\n--- Phase 3: {'COMMIT' if success else 'ROLLBACK'} ---")
    
    # Commit or rollback all transactions
    for shard_key, conn in connections.items():
        try:
            if success:
                conn.commit()
                print(f"  [{shard_key}] COMMIT")
            else:
                conn.rollback()
                print(f"  [{shard_key}] ROLLBACK")
        except Exception as e:
            print(f"  [{shard_key}] Error: {e}")
        finally:
            conn.close()
    
    print(f"\n{'='*60}")
    print(f"  Result: {'SUCCESS' if success else 'FAILED'}")
    print(f"{'='*60}")
    
    return success


def benchmark(mapping_file, insert_count, read_count, concurrency):
    """Run benchmark: inserts and reads"""
    print(f"\n{'='*60}")
    print("  CLI Tool: Benchmark")
    print(f"{'='*60}")
    
    mapping = load_mapping(mapping_file)
    
    print(f"\nConfiguration:")
    print(f"  - Inserts: {insert_count}")
    print(f"  - Reads: {read_count}")
    print(f"  - Concurrency: {concurrency}")
    print(f"  - Shards: {len(set(mapping.values()))}")
    
    # Connection pool per shard
    pools = {}
    for shard_key, conn_str in mapping.items():
        if conn_str not in pools:
            params = parse_connection_string(conn_str)
            pools[conn_str] = {
                'params': params,
                'lock': threading.Lock()
            }
    
    def get_connection(conn_str):
        params = pools[conn_str]['params']
        return psycopg2.connect(
            host=params.get('host', 'localhost'),
            port=int(params.get('port', 5432)),
            database=params.get('database'),
            user=params.get('username'),
            password=params.get('password')
        )
    
    inserted_ids = []
    insert_lock = threading.Lock()
    
    def do_insert(_):
        plan_id = str(uuid.uuid4())
        shard_key = get_shard_key(plan_id)
        conn_str = mapping[shard_key]
        
        conn = get_connection(conn_str)
        try:
            cursor = conn.cursor()
            cursor.execute(
                "INSERT INTO travel_plans (id, title) VALUES (%s, %s)",
                (plan_id, f"Plan {plan_id[:8]}")
            )
            conn.commit()
            with insert_lock:
                inserted_ids.append(plan_id)
            return True
        except Exception as e:
            conn.rollback()
            return False
        finally:
            conn.close()
    
    def do_read(_):
        if not inserted_ids:
            return False
        with insert_lock:
            plan_id = random.choice(inserted_ids)
        shard_key = get_shard_key(plan_id)
        conn_str = mapping[shard_key]
        
        conn = get_connection(conn_str)
        try:
            cursor = conn.cursor()
            cursor.execute(
                "SELECT id, title, created_at FROM travel_plans WHERE id = %s",
                (plan_id,)
            )
            result = cursor.fetchone()
            return result is not None
        except Exception:
            return False
        finally:
            conn.close()
    
    # Run inserts
    print(f"\n--- INSERT Benchmark ---")
    start_time = time.time()
    
    with ThreadPoolExecutor(max_workers=concurrency) as executor:
        futures = [executor.submit(do_insert, i) for i in range(insert_count)]
        success_count = sum(1 for f in as_completed(futures) if f.result())
    
    insert_duration = time.time() - start_time
    insert_ops = insert_count / insert_duration
    
    print(f"  Total: {insert_count} operations")
    print(f"  Success: {success_count}")
    print(f"  Duration: {insert_duration:.2f} seconds")
    print(f"  Throughput: {insert_ops:.1f} ops/sec")
    
    # Run reads
    print(f"\n--- READ Benchmark ---")
    start_time = time.time()
    
    with ThreadPoolExecutor(max_workers=concurrency) as executor:
        futures = [executor.submit(do_read, i) for i in range(read_count)]
        success_count = sum(1 for f in as_completed(futures) if f.result())
    
    read_duration = time.time() - start_time
    read_ops = read_count / read_duration
    
    print(f"  Total: {read_count} operations")
    print(f"  Success: {success_count}")
    print(f"  Duration: {read_duration:.2f} seconds")
    print(f"  Throughput: {read_ops:.1f} ops/sec")
    
    # Summary
    total_duration = insert_duration + read_duration
    print(f"\n{'='*60}")
    print(f"  BENCHMARK COMPLETE")
    print(f"  Total time: {total_duration:.2f} seconds")
    print(f"  INSERT: {insert_ops:.1f} ops/sec")
    print(f"  READ: {read_ops:.1f} ops/sec")
    print(f"{'='*60}")


def main():
    parser = argparse.ArgumentParser(description='PostgreSQL Sharding CLI Tool')
    subparsers = parser.add_subparsers(dest='command', help='Commands')
    
    # apply-sql command
    apply_parser = subparsers.add_parser('apply-sql', help='Apply SQL to all shards')
    apply_parser.add_argument('--mapping', required=True, help='Path to mapping.json')
    apply_parser.add_argument('--file', required=True, help='Path to SQL file')
    
    # benchmark command
    bench_parser = subparsers.add_parser('benchmark', help='Run benchmark')
    bench_parser.add_argument('--mapping', required=True, help='Path to mapping.json')
    bench_parser.add_argument('--count', type=int, default=1000, help='Number of inserts')
    bench_parser.add_argument('--reads', type=int, default=1000, help='Number of reads')
    bench_parser.add_argument('--concurrency', type=int, default=10, help='Concurrent threads')
    
    args = parser.parse_args()
    
    if args.command == 'apply-sql':
        apply_sql(args.mapping, args.file)
    elif args.command == 'benchmark':
        benchmark(args.mapping, args.count, args.reads, args.concurrency)
    else:
        parser.print_help()


if __name__ == '__main__':
    main()
