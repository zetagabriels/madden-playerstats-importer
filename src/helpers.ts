import { promisify } from 'util';
import { writeFile } from 'fs';
import Player from './models/player';

export const writeFileAsync = promisify(writeFile);

export function write(str: string): boolean {
    return process.stdout.write(str);
}

export function writeLine(line: string): boolean {
    return write(line + '\n');
}

export function generateUrl(year: number | string, type: 'passing' | 'rushing' | 'receiving' | 'defense'): string {
    return `https://www.pro-football-reference.com/years/${year}/${type}.htm`;
}

export function instantiate<T>(p: new (json: object) => T, json: object): T {
    return new p(json);
}

export type PlayerConstructor<T extends Player> = new (json: any) => T;

class P<T extends Player> {
    data: T;
    constructor(ctor: PlayerConstructor<T>, json: any) {
        this.data = new ctor(json);
    }
}