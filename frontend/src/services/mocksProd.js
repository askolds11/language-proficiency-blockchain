// ToDo: these are temporary mocks for production while we wait for the real API

const autocompleteTestData = [
  {
    id: 'jp',
    name: 'League of Legends',
  },
  {
    id: 'ar',
    name: 'Fallout 4',
  },
  {
    id: 'br',
    name: 'The Witcher 3: Wild Hunt',
  },
  {
    id: 'lv',
    name: 'Counter-Strike: Global Offensive',
  },
  {
    id: 'mt ',
    name: 'Dishonored ',
  },
  {
    id: 'ma ',
    name: 'Dota 2 ',
  },
  {
    id: 'jp',
    name: 'Pokémon Black and White',
  },
  {
    id: 'rus',
    name: 'Übersoldier',
  },
];

const autocompletePreselected = autocompleteTestData[1].id;
const autocompletePreselectedFunc = autocompleteTestData[2];

export default {
  autocompleteTestData,
  autocompletePreselected,
  autocompletePreselectedFunc,
};
