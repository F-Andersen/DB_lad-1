import { sleep } from 'k6';
import { createTravelPlan } from './utils/api-client.js';
import { generateTravelPlan } from './utils/data-generator.js';

export const options = {
    stages: [
        { duration: '2m', target: 50 },
        { duration: '2m', target: 100 },
        { duration: '2m', target: 200 },
        { duration: '2m', target: 300 },
        { duration: '5m', target: 0 },
    ],
};

export default function () {
    createTravelPlan(generateTravelPlan());
    sleep(1);
}
