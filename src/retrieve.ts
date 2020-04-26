import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import { Observable, of } from 'rxjs';
import { retry, switchMap } from 'rxjs/operators';
import { write } from './helpers';
import Player from './models/player';

/*
 * TODO
 *
 * Need to convert *each type* of player based on the webpage requested.
 * Each has completely unique fields, outside the ones in Player.ts
 * Need to genericize the "convertToPlayerArray" to only grab those fields, then pass to the proper class
 *
*/

function getWebpage(url: string): Observable<CheerioStatic> {
  return RxHR.get(url).pipe(
    retry(1),
    switchMap(response => of(cheerio.load(response.body))),
  );
}

function createInstance<T extends Player>(p: new (json: any) => T, json: any): T {
  return new p(json);
}

function convertToPlayerArray<T extends Player>($: CheerioStatic): Observable<T[]> {
  const players: T[] = [];
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

    const player = createInstance(T, json);
    players.push(player);
  });

  write('Retrieved and parsed all players from remote source.');
  return of(players);
}

export default function getRemoteData<T extends Player>(url: string): Observable<T[]> {
  return getWebpage(url).pipe(
    switchMap($ => convertToPlayerArray<T>($))
  );
}