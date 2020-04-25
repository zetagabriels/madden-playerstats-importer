import { writeLine, write, generateUrl } from './helpers';
import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import getopts from 'getopts';
import path from 'path';
import { Observable, of } from 'rxjs';
import { retry, map, switchMap, tap } from 'rxjs/operators'
import Player from './models/player';

function getWebpage(url: string): Observable<CheerioStatic> {
    return RxHR.get(url).pipe(
        retry(1),
        switchMap(response => of(cheerio.load(response.body))),
    );
}

function convertToPlayerArray($: CheerioStatic): Player[] {
    const players: Player[] = [];
    $('tbody > tr:not(.thead)').each((_, tr) => {
        const $tr = $(tr);
        const json = $tr.children().toArray().map((td) => {
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

        const player = Player.convert(json);
        players.push(player);
    });

    return players;
}

function main(): void {
    const opts = getopts(process.argv.slice(2), {
        alias: {
            help: 'h',
            year: 'y',
        },
        default: {
            year: new Date().getFullYear() - 1
        },
    });

    const url = generateUrl(opts.year, 'passing');
    getWebpage(url).subscribe(convertToPlayerArray);
}

main();