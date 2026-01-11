import { sleep } from 'k6';
import {
    createTravelPlan,
    getTravelPlanById,
    updateTravelPlan,
    deleteTravelPlan,
    createLocation
} from './utils/api-client.js';

import { generateTravelPlan, generateLocation } from './utils/data-generator.js';
import { extractIdOrFail } from './utils/response-utils.js';

export const options = {
    vus: 1,
    duration: '1m',
};

export default function () {
    const planRes = createTravelPlan(generateTravelPlan());
    const planId = extractIdOrFail(planRes, 'CreateTravelPlan');

    createLocation(planId, generateLocation());

    getTravelPlanById(planId);

    updateTravelPlan(planId, {
        title: 'Updated Plan',
        budgetEur: 1234,
        version: 1
    });

    deleteTravelPlan(planId);

    sleep(1);
}
