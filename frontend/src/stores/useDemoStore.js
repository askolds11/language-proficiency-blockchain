import { defineStore } from 'pinia';

export const useDemoStore = defineStore('demo', {
  state: () => ({
    filterData: [
      {
        id: 'randomFilter',
        name: 'Šis ir nejaušs filtrs',
      },
      {
        id: 'alsoRandomFilter',
        name: 'Un arī šis',
      },
      {
        id: 'randomFilter2',
        name: 'Un šis...',
      },
      {
        id: 'guessRandomFilter',
        name: 'Uzmini, kāds ir filtrs?',
      },
      {
        id: 'relentlessFilter',
        name: 'Oho, tu esi tik ziņkārīgs',
      },
      {
        id: 'notRandomFilter',
        name: '!Nav_nejaušs_filtrs',
      },
    ],
    videoGames: [
      {
        id: 1,
        name: 'League of Legends',
        popularity: 153109020,
        description:
          'League of Legends ir ļoti populāra daudzspēlētāju tiešsaistes kaujas arēna (MOBA) spēle, ko izstrādā Riot Games. Spēlētāji izvēlas unikālus čempionus ar dažādām spējām, strādā komandās, lai cīnītos viens pret otru, izstrādātu stratēģiju un iznīcinātu ienaidnieku Nexus, lai gūtu uzvaru dinamiskā un konkurētspējīgā virtuālajā kaujas laukā.',
        multiplayer: true,
        rating: 4.5,
        country: 'us',
        group: '1',
        clickable: true,
        category: 'red',
      },
      {
        id: 2,
        name: 'Fallout 4',
        popularity: 1822764.41,
        description:
          'Fallout 4 ir darbības lomu spēle postapokaliptiskā atvērtās pasaules vidē, kurā spēlētāji pārvietojas pa tuksnesi, iesaistās kaujās un pieņem kritiskus lēmumus, kas veido sava rakstura ceļa iznākumu.',
        multiplayer: false,
        rating: 4.0,
        country: 'us',
        group: '2',
        clickable: true,
        href: 'list',
        category: 'red',
      },
      {
        id: 3,
        name: 'The Witcher 3: Wild Hunt',
        popularity: 16276.2,
        description:
          'Witcher 3 ir episka darbība RPG, ko izstrādājis CD Projekt Red, ņemot vērā Rivijas Džeralta piedzīvojumus, kas ir monstru mednieks, kurš tiek pieņemts darbā, plašā un krāšņi atklātā pasaulē, kas pilna ar aizraujošiem lūgumiem, tikumiskām izvēlēm un elpu aizraujošām ainavām.',
        multiplayer: false,
        rating: 4.8,
        country: 'pl',
        group: '2',
        href: 'list',
      },
      {
        id: 4,
        name: 'Counter-Strike: Global Offensive',
        popularity: 877877.2,
        description:
          'CSGO ir konkurētspējīgs pirmās personas šāvējs, kurā spēlētāji pievienojas vai nu teroristu, vai pretterorisma komandai, iesaistoties spraigos mačos, lai pabeigtu mērķus, ielaistu vai atvairītu bumbas, un demonstrējot savas taktiskās šaušanas prasmes ātrslidošanā.',
        multiplayer: true,
        rating: 4.6,
        country: 'us',
        group: '3',
        href: 'list',
        category: 'red',
      },
      {
        id: 5,
        name: 'Dishonored',
        popularity: 527,
        description:
          'Dishonored, tā ir slepena spēle tumšā, industriālā pasaulē, kur spēlētāji uzņemas Korvo Attano lomu, prasmīgs slepkava, meklēdams atriebību. Apbruņojušies ar pārdabiskām spējām un nāvējošu sīkrīku klāstu, spēlētāji var izvēlēties būt nāvējoši vai ne-nāvējoši, pārvietojoties sarežģītos līmeņos, atšķetinot politiskās sazvērestības un veidojot Dunwall pilsētas likteni.',
        multiplayer: false,
        rating: 4.4,
        country: { id: 'us', name: 'Amerikas Savienotās Valstis' },
        group: '4',
      },
      {
        id: 6,
        name: 'Dota 2',
        popularity: 422656.8,
        description:
          'DOTA 2 ir slavena daudzspēlētāju tiešsaistes kaujas arēna (MOBA) spēle no Valves, kurā spēlētāji kontrolē unikālus varoņus ar spēcīgām spējām, komandu, lai iznīcinātu ienaidnieka seno, un paļaujas uz stratēģisko komandas darbu un individuālo prasmi gūt uzvaras dinamiskos mačos.',
        multiplayer: true,
        rating: 4.7,
        country: 'us',
        group: '1',
        clickable: true,
        category: 'red',
      },
      {
        id: 7,
        name: 'Half-Life',
        popularity: 548,
        description:
          'Half-Life ir neglābjams pirmās personas šāvējs, ko izstrādājis Valve, pazīstams ar savu iegremdējošo stāstījumu, novatorisko spēles spēli un neaizmirstamiem provokatoriem Gordonu Frīmenu, kurš cīnās pret starpdimensiju draudiem un valdības spēkiem atmosfēras un saķeršanās piedzīvojumā.',
        multiplayer: false,
        rating: 4.7,
        country: 'us',
        group: '3',
        href: 'list',
      },
      {
        id: 8,
        name: 'Minecraft',
        popularity: 163674756,
        description:
          'Minecraft ir smilšu kastes izdzīvošanas spēle, kas dod iespēju spēlētājiem veidot un izpētīt blokainas, procedurally radītas pasaules, radīt struktūras, amatnieciskus priekšmetus un piedzīvot nebeidzamas radošas iespējas apburošā, pikelētā Visumā.',
        multiplayer: true,
        rating: 4.9,
        country: 'swe',
        group: '4',
      },
      {
        id: 9,
        name: 'GTA V',
        popularity: 115880.6,
        description:
          'GTA V ir ar darbību komplektēta brīvdabas spēle, ko izstrādā Rockstar Games, kurā spēlētāji var izpētīt plašo Los Santu pilsētu, piedalīties aizraujošos heistos, iesaistīties dažādās noziedzīgās aktivitātēs un pārslēgties no trim atšķirīgiem provokatoriem uz aizraujošu un dinamisku stāstu vadītu pieredzi.',
        multiplayer: true,
        rating: 4.8,
        country: { id: 'gb', name: 'Apvienotā karaliste' },
        group: '4',
      },
      {
        id: 10,
        name: 'Pokémon Black and White',
        popularity: 15640000,
        description:
          '“Pokémon Black and White” ir piektās paaudzes Pokémon spēles, kas notiek Unova reģionā un kas piedāvā jaunu ķeramo un apmācāmo radību pulku. Ar aizraujošu stāstu, atjauninātām grafikām un jaunu spēļu mehāniku viņi ieviesa dinamisku sezonas sistēmu un uzsvēra patiesības un ideālu motīvus, piedāvājot spēlētājiem jaunu pieredzi klasiskajā Pokémon.',
        multiplayer: false,
        rating: 4.4,
        country: 'jp',
        group: '4',
      },
      {
        id: 11,
        name: 'Übersoldier',
        popularity: 392,
        description:
          '“Übersoldier” ir Otrā pasaules kara pirmās personas šāvēja videospēle, kas seko vācu karavīram, kurš pēc ievainošanas kļūst par eksperimentālu superkaravīru. Izmantojot alternatīvu vēsturi, spēle apvieno ar darbību saistītas spēles un stāstu par intrigām un pārdabiskiem elementiem, spēlētājiem pārvietojoties pa kara plosītām ainavām.',
        multiplayer: false,
        rating: 2.8,
        country: 'rus',
        group: '3',
      },
    ],
    testData: {
      firstName: 'Gabriel',
      lastName: 'Fernandez',
      favGame: 10,
      birthDate: '1990-09-10',
      birthCountry: 'Brazīlija',
      title: 'Pokémon TCG Seniors Pasaules Čempions 2023',
      achievements: [
        {
          id: 1,
          date: '2023-08-13',
          place: 1,
          tier: 'S-Tier',
          name: '2023 Pokémon World Championships - TCG (Seniors)',
        },
        {
          id: 2,
          date: '2023-06-04',
          place: 2,
          tier: 'B-Tier',
          name: '2023 Pokémon Santiago Regional Championships - TCG (Seniors)',
        },
        {
          id: 3,
          date: '2023-05-06',
          place: 1,
          tier: 'B-Tier',
          name: '2023 Pokémon São Paulo Regional Championships - TCG (Seniors)',
        },
        {
          id: 4,
          date: '2022-01-27',
          place: 2,
          tier: 'Weekly',
          name: 'AFD Weekly #11',
        },
        {
          id: 5,
          date: '2021-10-12',
          place: 1,
          tier: 'Weekly',
          name: 'Late Night Series #10',
        },
        {
          id: 6,
          date: '2021-10-04',
          place: 1,
          tier: 'Weekly',
          name: 'Weekly Standard Tournament #37',
        },
        {
          id: 7,
          date: '2021-09-20',
          place: 1,
          tier: 'Weekly',
          name: 'Weekly Standard Tournament #36',
        },
        {
          id: 8,
          date: '2021-09-15',
          place: 2,
          tier: 'Weekly',
          name: 'Weekly Standard Tournament #7',
        },
      ],
      description:
        'Gabriels Fernandez ir kļuvis par 2023. gada Pokémon TCG Seniors pasaules čempionu, un tas ir neapšaubāmi monumentāls sasniegums. Lai iegūtu šo prestižo titulu, viņš ir parādījis gan dziļu spēles mehānikas izpratni, gan spēju veiksmīgi paredzēt un novērst pretinieku stratēģijas. Viņa ceļš uz pasaules čempionātu bija pilns ar izaicinājumiem, prasot no viņa milzīgas prasmes, praksi un veltījumu. Gabriela triumfs ne tikai ir personīgs sasniegums, bet arī lieliska iedvesma jauniem spēlētājiem, kuri vēlas izcelties Pokémon TCG arenā.',
    },
    today: new Date(),
    testDate: new Date(2023, 9, 1),
  }),
  actions: {
    updateData(newData) {
      this.videoGames = newData;
    },
    updateFiltersData(newData) {
      this.filterData = newData;
    },
  },
  getters: {
    videoGamesComputed: (state) =>
      state.videoGames.map((game) => ({
        id: game.id,
        name: game.name,
      })),
    videoGamesComputedCountry: (state) =>
      state.videoGames.map((game) => ({
        id: game.id,
        name: game.name,
        country: game?.country,
      })),
  },
});
