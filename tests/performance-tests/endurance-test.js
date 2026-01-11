import { sleep } from 'k6';
import { createTravelPlan } from './utils/api-client.js';
import { generateTravelPlan } from './utils/data-generator.js';

export const options = {
    stages: [
        { duration: '5m', target: 20 },
        { duration: '30m', target: 20 },
        { duration: '5m', target: 0 },
    ],
};

export default function () {
    createTravelPlan(generateTravelPlan());
    sleep(1);
}
