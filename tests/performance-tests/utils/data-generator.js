const ADJECTIVES = ['Exciting', 'Relaxing', 'Amazing', 'Sunny', 'Historic', 'Snowy', 'Romantic'];
const CITIES = ['Kyiv', 'Sumy', 'Lviv', 'Odesa', 'Kharkiv', 'Dnipro', 'Ivano-Frankivsk'];
const LOC_TYPES = ['Museum', 'Park', 'Beach', 'Castle', 'Cafe', 'Monument', 'Square'];

function randomItem(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
}

export function generateTravelPlan() {
    const adjective = randomItem(ADJECTIVES);
    const city = randomItem(CITIES);
    return {
        title: `${adjective} ${city} Trip`,
        budgetEur: Math.floor(Math.random() * 8000) + 500,
    };
}

export function generateLocation() {
    const name = `${randomItem(LOC_TYPES)} ${Math.floor(Math.random() * 100)}`;
    return {
        name,
        budgetEur: Math.floor(Math.random() * 2000) + 50,
        notes: 'Generated location for performance testing',
    };
}
