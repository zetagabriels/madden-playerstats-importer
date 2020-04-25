export default class Player {
    rank!: number;
    name!: string;
    team!: string;
    position!: string;

    public static convert(json: any): Player {
        const p = new Player();
        return p;
    }
}