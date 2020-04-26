import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import { Observable, of } from 'rxjs';
import { retry, switchMap } from 'rxjs/operators';
import { writeLine } from './helpers';
import Player from './models/player';

function getWebpage(url: string): Observable<CheerioStatic> {
  return RxHR.get(url).pipe(
    retry(1),
    switchMap(response => of(cheerio.load(response.body))),
  );
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

    const player = (new Player() as T);
    player.convert(json);
    // console.log(JSON.stringify(player));
    players.push(player);
  });

  writeLine('Retrieved and parsed all players from remote source.');
  return of(players);
}

export default function getRemoteData<T extends Player>(url: string): Observable<T[]> {
  return getWebpage(url).pipe(
    switchMap($ => convertToPlayerArray<T>($))
  );
}
