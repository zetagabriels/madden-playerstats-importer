export function write(str: string) {
    process.stdout.write(str);
}

export function writeLine(line: string) {
    return write(line + '\n');
}

export function generateUrl(year: number | string, type: 'passing' | 'rushing' | 'defense') {
    return `https://www.pro-football-reference.com/years/${year}/${type}.htm`;
}