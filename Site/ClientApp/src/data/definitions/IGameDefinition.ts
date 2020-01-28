import { ITechTree } from './ITechTree';
import { IBattlefield } from '../instances/IBattlefield';

export interface IGameDefinition {
    techTree: ITechTree;
    battlefield: IBattlefield;
}
