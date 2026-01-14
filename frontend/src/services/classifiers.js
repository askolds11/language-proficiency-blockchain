import mocksProd from '@/services/mocksProd';

export function getAutocompleteTest(query) {
  // wait 4 seconds to simulate slow response
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        status: 200,
        data: mocksProd.autocompleteTestData.filter((item) =>
          item.name.toLowerCase().includes(query.toLowerCase())
        ),
      });
    }, 1000);
  });
}
