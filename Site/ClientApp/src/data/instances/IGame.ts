import { ITechTree } from '../definitions/ITechTree';
import { IBattlefield } from './IBattlefield';

export interface IGame {
    techTree: ITechTree;
    battlefield: IBattlefield;
    localPlayerID?: number;
}
