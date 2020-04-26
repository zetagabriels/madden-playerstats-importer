import Player from './player';

export default class ReceivingPlayer extends Player {


  public static convert(json: any): ReceivingPlayer {
    const p = super.convert(json) as ReceivingPlayer;
    return p;
  }
}