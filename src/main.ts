import { writeLine, write, generateUrl } from './helpers';
import cheerio from 'cheerio';
import { RxHR } from '@akanass/rx-http-request';
import getopts from 'getopts';
import path from 'path';
import { Observable } from 'rxjs';
import { retry } from 'rxjs/operators'
import { writeFileSync } from 'fs';

function getWebpage(url: string): Observable<any> {
    return RxHR.get(url).pipe(
        retry(1)
    );
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
    getWebpage(url).subscribe(data => {
        const $ = cheerio.load(data.body);
        $('tbody > tr:not(.thead)').each((i, el) => {
            const $el = $(el);
            console.log($el.text());
        });
    });
}

main();