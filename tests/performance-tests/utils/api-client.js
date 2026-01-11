import http from 'k6/http';
import { check } from 'k6';

const BASE_URL = __ENV.API_URL || 'https://localhost:6061';

const jsonHeaders = {
    'Content-Type': 'application/json',
};

export function createTravelPlan(payload) {
    const res = http.post(
        `${BASE_URL}/api/TravelPlans`,
        JSON.stringify(payload),
        {
            headers: jsonHeaders,
            responseType: 'text',
        }
    );

    check(res, {
        'create-plan status 201': (r) => r.status === 201,
    });

    return res;
}

export function createLocation(planId, payload) {
    return http.post(
        `${BASE_URL}/api/TravelPlans/${planId}/locations`,
        JSON.stringify(payload),
        {
            headers: jsonHeaders,
            responseType: 'text',
        }
    );
}

export function getTravelPlanById(id) {
    return http.get(`${BASE_URL}/api/TravelPlans/${id}`);
}

export function updateTravelPlan(id, payload) {
    return http.put(
        `${BASE_URL}/api/TravelPlans/${id}`,
        JSON.stringify(payload),
        {
            headers: jsonHeaders,
            responseType: 'text',
        }
    );
}

export function deleteTravelPlan(id) {
    return http.del(`${BASE_URL}/api/TravelPlans/${id}`);
}

export function deleteLocation(id) {
    return http.del(`${BASE_URL}/api/Locations/${id}`);
}
