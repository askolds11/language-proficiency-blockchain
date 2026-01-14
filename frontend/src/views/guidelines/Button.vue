<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher, LxButton } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'button' });
  }
}
</script>
<template>
  <LxContentSwitcher
    :items="[
      { id: 'sandbox', name: 'Smilškaste' },
      { id: 'guidelines', name: 'Vadlīnijas' },
    ]"
    v-model="routerValue"
    @click="contentSwitcherClick"
  />
  <article class="lx-article">
    <h2>Pamati</h2>
    <p>
      Pogas izmanto, lai izpildītu darbības sistēmā. Normālā gadījumā, jebkurš klikšķināms skata
      elements, kas izraisa darbību biznesa procesa darbību (navigāciju, izmaiņas datos vai
      kontekstā) ir poga (teksta ievadlauks nav poga - klikšķis pārvieto tajā tastatūras kursoru,
      izvēles rūtiņa ir poga - klikšķis tajā maina lauka vērtību).
    </p>
    <p>Pogas sastāv no ikonas un/vai birkas un vienmēr satur paskaidri.</p>
    <p>Gan birkai, gan ikonai jāapzīmē tā darbība, ko izpildīs pogas nospiešana.</p>
    <p>
      Pogai birkai un paskaidrei vienmēr jābūt noformētai kā darbības vārdam, kas apraksta darbību,
      kas izpildīsies, kad pogu nospiedīs. Izņēmums: kad pogas tiek izmantotas kā atbildes varianti
      uz jautājumu, kas ir nepārprotami skaidrs. Šādā gadījumā paskaidrei joprojām jāsatur darbības
      vārds (piemēram, balsošanas pogām var izmantot birkas "Par" un "Pret", ja no konteksta ir
      skaidrs, ka tās ir balsojuma izvēles. Paskaidrēm šādā gadījumā jābūt "Balsot par" un "Balsot
      pret")
    </p>
    <p>
      Dialoga skata pogām jābūt nepārprotami saprotamām, bet maksimāli īsām, jāizvairās no birkām
      "Jā" un "Nē". Ja dialogs uzdod jautājumu, tad darbību pogām jāatbild uz to jautājumu:
    </p>
  </article>
  <article class="lx-article">
    <h3 class="lx-bad-example">Slikti</h3>
    <div class="lx-region lx-bad-example">
      <p>Vai tiešām negribat atcelt darbību?</p>
      <div class="lx-button-set buttons-3">
        <LxButton label="Jā" />
        <LxButton label="Nē" />
        <LxButton label="Atcelt" />
      </div>
    </div>
  </article>
  <article class="lx-article">
    <h3 class="lx-bad-example">Slikti</h3>
    <div class="lx-region lx-bad-example">
      <p>Vai tiešām gribat saglabāt dokumentu?</p>
      <div class="lx-button-set buttons-3">
        <LxButton label="Jā" />
        <LxButton label="Nē" />
        <LxButton label="Atcelt" />
      </div>
    </div>
  </article>

  <article class="lx-article">
    <h3 class="lx-good-example">Labi</h3>
    <div class="lx-region lx-good-example">
      <p>Vai tiešām gribat saglabāt dokumentu?</p>
      <div class="lx-button-set buttons-3">
        <LxButton label="Saglabāt" />
        <LxButton label="Nesaglabāt" />
        <LxButton label="Atcelt" kind="secondary" />
      </div>
    </div>
  </article>

  <article class="lx-article">
    <p>Pogas ar birkām ir vienmēr labāk lietojamas, nekā pogas bez birkām.</p>

    <p>
      Poga bez ikonas ir labāka par pogu ar nesaprotamu ikonu, tāpēc jācenšās vienādām, vai
      vienlīdzīgām darbībām izmantot dizaina valodā definētās vai vispārīgi pieņemtās ikonas.
    </p>
    <p>Pogas ar ikonām, bet bez birkām drīkst izmantot tikai šādos gadījumos:</p>
    <ul>
      <li>
        Kad pogas darbība normālajā gadījumā šajā kontekstā ir viennozīmīgi saprotama (piemēram,
        dzēšanas pogas ar "miskastes" ikonu būs saprotama 80% gadījumu, pārējos 20%, lietotājs varēs
        pārbaudīt pogas nozīmi paskaidrē. Jāpaļaujās, ka ikonu, kas definētas dizaina valodā un
        pielietotas pareizi, iekļaujas 80%);
      </li>
      <li>
        Kad pogas ar pilno nosaukumu birkā izvietojums vienā skatā nav iespējams telpas ierobežojumu
        dēļ (piemēram, samazināto izmēru skatos). Pogas [[ikona]] jāuztver kā papildus "āķis"
        skenēšanas atvieglošanai. Tas nozīmē, ka izvēloties pogas ikonu, lēmums ir jāvertē ne tikai
        vienas pogas, bet visa skata kontekstā.
      </li>
    </ul>

    <p>
      Piemērs 1. Ja skatā ir jāattēlo vienā grupā trīs pievienošanas pogas, tad vienādas
      pievienošanas ikonas izmantošanai katrā no tām nav jēgas - skenēšanu tas neatvieglo un
      lietotājam tik un tā nāksies izmantot pogas birka, lai atšķirtu vienu pogu no otras.
    </p>
    <div class="lx-region">
      <div class="lx-button-set lx-vertical">
        <LxButton label="Pievienot fizisku personu" icon="person" kind="secondary" />
        <LxButton label="Pievienot juridisku personu" icon="institution" kind="secondary" />
        <LxButton label="Pievienot grupu" icon="team" kind="secondary" />
      </div>
    </div>
    <p>
      Piemērs 2. Tajā pašā laikā, ja pogu grupā ir 5 dažādu darbību pogas un 3 no tām ir dažāda
      veida personu pievienošana, tad visām trīs pogām varētu izmantot vienādas ikonas, lai tās
      vizuāli sagrupētu un atvieglotu to atrašanu.
    </p>
    <div class="lx-region">
      <div class="lx-button-set lx-vertical">
        <LxButton label="Pievienot fizisku personu" icon="add-item" kind="secondary" />
        <LxButton label="Pievienot juridisku personu" icon="add-item" kind="secondary" />
        <LxButton label="Pievienot grupu" icon="add-item" kind="secondary" />
        <LxButton label="Augšupielādēt personas" icon="upload" kind="secondary" />
        <LxButton label="Pārlādēt sarakstu" icon="refresh" kind="secondary" />
      </div>
    </div>
    <p>
      Pogas nospiešanai jāizraisa momentāla sistēmas reakcija (reakcija, kas ir ātrāka par 200ms
      normālajā gadījumā). Tas nenozīmē (kaut arī ir vēlams), ka pogas nospiešanai momentāli
      jāizpilda pogas darbība, taču lietotājam jāsaprot, ka pogas nospiešana bija veiksmīga.
      Piemēram, ja pogas nospiešana izraisa dialoga atvēršanu, kurā attēlosies no datu bāzes
      pieprasītie dati, tad dialogs jāatver momentāli, bet dati jāatlasa asinhroni, attēlojot
      loaderi. Ja pogas darbība izpildās fonā un lietotājs paliek tajā pašā skatā, vai nav iespējams
      reaģēt momentāli citu iemeslu dēļ, tad jāizmanto pogas "progresa skats" darbības laikā:
    </p>
  </article>
  <article class="lx-article">
    <h2>Pogu izkārtojums</h2>
    <p>Ja vairākas pogas tiek izkārtotas vienā grupā, tad īpaša uzmanība jāpievērš to secībai.</p>
    <p>
      Rezultatīvās un pozitīvās darbības, ko lieto visbiežāk, izkārto pa kreisi (horizontālā
      izkārtojuma gadījumā) un augstāk (vertikālā izkārtojuma gadījumā) par pārējām, atbilstoši
      Virziena vadlīnijām. Primārā darbība normālajā gadījumā vienmēr ir pirmā savā grupā (izņemot
      gadījumus, kad loģika diktē savādāku secību: piemēram, ja grupā ir pogas "Jā" un "Nē", bet
      "Nē" ir prioritārā darbība, tad primārā poga ir otrā savā grupā).
    </p>
    <p>
      Atcelšanas darbība (nerezultatīvā aizvēršana, izmaiņu atcelšana) vienmēr ir pēdējā sarakstā un
      nekad nav primārā poga.
    </p>
    <p>Pārējās pogas jāizkārto starp pamata pozitīvām darbībām un atcelšanas darbībām.</p>
    <h2>Pogu veidi</h2>
    <p>Atkarībā no izvēlētās dizaina valodas, definē 4 veidu pogas:</p>
    <ul>
      <li><strong>Primārā</strong> poga (pirmās prioritātes poga);</li>
      <li><strong>Sekundārā</strong> poga (otrās prioritātes poga);</li>
      <li>
        <strong>Palīg-</strong>, jeb <strong>terciārā</strong> poga (trešās prioritātes poga);
      </li>
      <li><strong>"Spoku"</strong> poga (zemākās prioritātes poga);</li>
    </ul>
    <div class="lx-region">
      <div class="guidelines-pic">
        <div class="guidelines-button-types">
          <p>Primārās pogas</p>
          <LxButton icon="save" />
          <LxButton icon="save" label="Saglabāt" />
          <LxButton label="Saglabāt" />
        </div>
        <div class="guidelines-button-types">
          <p>Sekundārās pogas</p>
          <LxButton icon="save" kind="secondary" />
          <LxButton icon="save" label="Saglabāt" kind="secondary" />
          <LxButton label="Saglabāt" kind="secondary" />
        </div>
        <div class="guidelines-button-types">
          <p>Terciārās pogas</p>
          <LxButton icon="save" kind="tertiary" />
          <LxButton icon="save" label="Saglabāt" kind="tertiary" />
          <LxButton label="Saglabāt" kind="tertiary" />
        </div>
        <div class="guidelines-button-types">
          <p>"Spoku" pogas</p>
          <LxButton icon="save" kind="ghost" />
          <LxButton icon="save" label="Saglabāt" kind="ghost" />
          <LxButton label="Saglabāt" kind="ghost" />
        </div>
      </div>
    </div>

    <p class="lx-warning">
      Jāizvairās no vienu un to pašu darbību izpildošu pogu izkārtošanas vienā skatā vairākos
      eksemplāros. Atkāpes no šī noteikuma ir pieļaujamas, ja konkrētajā pielietojumā tam ir labs
      iemesls - piemēram, gadījumos, kad lietotājs ir pieradis meklēt pogu vienā vietā, bet kaut
      kādu apstākļu dēļ poga ir jāpārvieto citur un pārvietošana drastiski ietekmē lietojamību, ir
      pieļaujams pārejas periodā nodublēt pogu, lai pāreja notiek vieglāk. Katrs no šādiem
      gadījumiem jāvērtē atsevišķi.
    </p>
    <p class="lx-warning">
      Jāizvairās no destruktīvo darbību pogu grupēšanas kopā ar apstiprinošo darbību pogām, lai
      <a
        target="_blank"
        rel="noopener noreferrer nofollow"
        href="https://intra.zzdats.lv/pages/viewpage.action?pageId=22857715"
      >
        mazinātu lietotāju kļūdas cenu</a
      >
      nejaušas nospiešanas gadījumā (atbilstošais pētījums:
      <a
        target="_blank"
        rel="noopener noreferrer nofollow"
        href="https://www.nngroup.com/articles/proximity-consequential-options/"
      >
        "Dangerous UX: Consequential Options Close to Benign Options"</a
      >).
    </p>
  </article>
  <article class="lx-article">
    <h2>Primārās pogas</h2>
    <p>
      Katram skatam (izņemot dispečerizācijas skatus) jādefinē viena, vai divas pamata darbības -
      kas ir tās darbības, deļ kurām lietotājs ir atnācis uz šo skatu, vai kas ir tās darbības, kas
      rezultatīvi pabeidz darbu ar šo skatu?
    </p>
    <p>Šadu darbību izpildei izmanto primārās pogas.</p>
    <p>
      Skati, kas satur vairākas līdzvērtīgas pamata darbības (piemēram, dispečerizācijas skati, vai
      jautājumu dialogu skati), drīkst saturēt vairāk par vienu primāro pogu, ar nosacījumu, ka
      primārās darbības ir tiešām līdzvērtīgas. Piemēram, dialogā ar jautājumu "Vai jūs tiešām
      gribat iziet no sistēmas?", varianti "Jā" un "Nē" nav līdzvērtīgi - atbildes "Jā" rezultāts ir
      daudz smagāks par "Nē", tajā pašā laikā, piemēram, balsošanas dialogā rezultāts "Par" un
      "Pret" ir līdzvērtīgi no procesa viedokļa un prioritizēt vienu vai otru nedrīkst.
    </p>
  </article>
  <article class="lx-article">
    <h2>Sekundārās pogas</h2>
    <p>Pārējo svarīgo darbību pogām jāizmanto sekundāras pogas.</p>
  </article>
  <article class="lx-article">
    <h2>Terciārās pogas</h2>
    <p>
      Terciārās pogas izmanto palīgdarbībām, kas pieder kādam skata elementam un attiecās tikai uz
      to (piemēram, saraksta elementa dzēšanas poga, vai visa skata rīkjosla), vai tehniskām
      darbībām, kas tiešā veidā uz biznesu procesu neattiecās (piemēram, elementu sakļaušanas
      pogām).
    </p>
    <p>
      Ja saraksta elements ir interaktīvs (izpilda kaut kādu darbību un nospiešanu), tas, vai tā
      daļa (piemēram, virsraksts) ir jānoformē kā terciāra poga.
    </p>
    <p>
      Ja starp terciāro pogām ir prioritārākas darbības par pārējām, to vietā drīkst izmantot
      sekundārās pogas.
    </p>
  </article>
  <article class="lx-article">
    <h2>Destruktīvās pogas</h2>
    <p>
      Destruktīvās darbības ir tādas darbības, kas izraisa neatgriezinisku vai grūti atgriezinisku
      (piemēram, darbība, kuru var atcelt aizejot uz citu skatu neskaitās atgrieziniska, bet
      saraksta elementa atvienošana, kad to elementu var pievienot pa jaunam šajā pašā formā - ir
      atgrieziniska) rezultātu
    </p>
    <p>
      Destruktīvo darbību pogām jāpiemēro īpašs stils (piemēram, ar atšķirīgu pogas hover-efektu).
    </p>
    <p>
      Šādām darbībām vienmēr jāseko vai nu pārprasīšanas dialogam, vai nu iespējai darbību atcelt
      tajā pašā skatā.
    </p>
    <p>Destruktīvo darbību pogas jācenšās negrupēt kopā ar pārējām darbībām.</p>
  </article>
</template>
