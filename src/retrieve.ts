import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import { Observable, of } from 'rxjs';
import { retry, switchMap } from 'rxjs/operators';
import { writeLine, generateUrl } from './helpers';
import DefensePlayer from './models/defense.player';
import PassingPlayer from './models/passing.player';
import ReceivingPlayer from './models/receiving.player';
import RushingPlayer from './models/rushing.player';

function getWebpage(url: string): Observable<CheerioStatic> {
  return RxHR.get(url).pipe(
    retry(1),
    switchMap(response => of(cheerio.load(response.body))),
  );
}

// the "convertToX" functions should be just one
// function with generic types
// but TS doesn't allow it
// all hail javascript

function convertToPassingPlayers($: CheerioStatic): Observable<PassingPlayer[]> {
  return parseWebpage($).pipe(
    switchMap(rows => {
      const arr: PassingPlayer[] = [];
      for (const jsonObj of rows) {
        const p = new PassingPlayer();
        p.convert(jsonObj);
        arr.push(p);
      }
      return of(arr);
    })
  );
}

function convertToDefensePlayers($: CheerioStatic): Observable<DefensePlayer[]> {
  return parseWebpage($).pipe(
    switchMap(rows => {
      const arr: DefensePlayer[] = [];
      for (const jsonObj of rows) {
        const p = new DefensePlayer();
        p.convert(jsonObj);
        arr.push(p);
      }
      return of(arr);
    })
  );
}

function convertToRushingPlayers($: CheerioStatic): Observable<RushingPlayer[]> {
  return parseWebpage($).pipe(
    switchMap(rows => {
      const arr: RushingPlayer[] = [];
      for (const jsonObj of rows) {
        const p = new RushingPlayer();
        p.convert(jsonObj);
        arr.push(p);
      }
      return of(arr);
    })
  );
}

function convertToReceivingPlayers($: CheerioStatic): Observable<ReceivingPlayer[]> {
  return parseWebpage($).pipe(
    switchMap(rows => {
      const arr: ReceivingPlayer[] = [];
      for (const jsonObj of rows) {
        const p = new ReceivingPlayer();
        p.convert(jsonObj);
        arr.push(p);
      }
      return of(arr);
    })
  );
}

function parseWebpage($: CheerioStatic): Observable<{ [key: string]: string | number }[]> {
  const rows: { [key: string]: string | number }[] = [];
  $('tbody > tr:not(.thead)').each((_, tr) => {
    const $tr = $(tr);
    const json: { [key: string]: string | number } = {};
    const list = $tr.children().toArray().map((td) => {
      const $td = $(td);
      let dataParsed: string | number;

      try {
        dataParsed = parseInt($td.text(), 10);
        if (isNaN(dataParsed)) throw Error();
      } catch {
        dataParsed = $td.text();
      }

      return {
        name: $td.data('stat'),
        value: dataParsed
      };
    });

    list.forEach(v => json[v.name] = v.value);
    rows.push(json);
  });

  writeLine('Retrieved and parsed all players from remote source.');
  return of(rows);
}

export function getPassingPlayers(year: string | number): Observable<PassingPlayer[]> {
  const url = generateUrl(year, 'passing');
  return getWebpage(url).pipe(
    switchMap($ => convertToPassingPlayers($))
  );
}

export function getDefensePlayers(year: string | number): Observable<DefensePlayer[]> {
  const url = generateUrl(year, 'defense');
  return getWebpage(url).pipe(
    switchMap($ => convertToDefensePlayers($))
  )
}
