import Player from './player';

export default class DefensePlayer extends Player {


  public static convert(json: any): DefensePlayer {
    const p = super.convert(json) as DefensePlayer;
    return p;
  }
}