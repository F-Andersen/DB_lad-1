import { sleep, fail } from 'k6';
import { createTravelPlan, createLocation } from './utils/api-client.js';
import { generateTravelPlan, generateLocation } from './utils/data-generator.js';

export const options = {
    stages: [
        { duration: '2m', target: 50 },
        { duration: '5m', target: 50 },
        { duration: '2m', target: 0 },
    ],
};

function getIdOrFail(res, name) {
    if (!res.body || res.body.length === 0) {
        fail(`${name}: empty body`);
    }
    const json = res.json();
    if (!json.id) {
        fail(`${name}: no id`);
    }
    return json.id;
}

export default function () {
    const planRes = createTravelPlan(generateTravelPlan());
    const planId = getIdOrFail(planRes, 'CreateTravelPlan');

    createLocation(planId, generateLocation());

    sleep(1);
}
