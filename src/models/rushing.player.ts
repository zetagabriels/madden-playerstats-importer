import Player from './player';

export default class RushingPlayer extends Player {


  public static convert(json: any): RushingPlayer {
    const p = super.convert(json) as RushingPlayer;
    return p;
  }
}