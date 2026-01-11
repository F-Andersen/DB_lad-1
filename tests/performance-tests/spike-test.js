import { sleep } from 'k6';
import { createTravelPlan } from './utils/api-client.js';
import { generateTravelPlan } from './utils/data-generator.js';

export const options = {
    stages: [
        { duration: '2m', target: 20 },
        { duration: '30s', target: 500 },
        { duration: '2m', target: 0 },
    ],
};

export default function () {
    createTravelPlan(generateTravelPlan());
    sleep(1);
}
