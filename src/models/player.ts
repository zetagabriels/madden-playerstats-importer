export default class Player {
    rank!: number;
    name!: string;
    team!: string;
    age!: number;
    position!: string;
    gamesPlayed!: number;
    gamesStarted!: number;
    qbRecord!: string;

    public static convert(json: any): Player {
        console.log(json);
        const p = new Player();
        p.rank = json.ranker;
        p.name = json.player;
        p.team = json.team;
        p.age = json.age;
        p.position = json.pos;
        p.gamesPlayed = json.g;
        p.gamesStarted = json.gs;
        p.qbRecord = json.qb_rec;

        return p;
    }
}