import { fail } from 'k6';

export function extractIdOrFail(res, actionName) {
    if (!res || !res.body) {
        fail(`${actionName}: response body is empty`);
    }

    let json;
    try {
        json = JSON.parse(res.body);
    } catch {
        fail(`${actionName}: response body is not valid JSON`);
    }

    if (!json.id) {
        fail(`${actionName}: response JSON does not contain id`);
    }

    return json.id;
}
