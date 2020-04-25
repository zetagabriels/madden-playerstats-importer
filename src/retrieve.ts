import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import { Observable, of } from 'rxjs';
import { retry, switchMap } from 'rxjs/operators';
import Player from './models/player';
import { write } from './helpers';

function getWebpage(url: string): Observable<CheerioStatic> {
    return RxHR.get(url).pipe(
        retry(1),
        switchMap(response => of(cheerio.load(response.body))),
    );
}

function convertToPlayerArray($: CheerioStatic): Observable<Player[]> {
    const players: Player[] = [];
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

        const player = Player.convert(json);
        players.push(player);
    });

    write('Retrieved and parsed all players from remote source.');
    return of(players);
}

export default function getRemoteData(url: string): Observable<Player[]> {
    return getWebpage(url).pipe(
        switchMap(convertToPlayerArray)
    );
}