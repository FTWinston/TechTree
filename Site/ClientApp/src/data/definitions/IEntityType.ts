import { IPurchasable } from './IPurchaseable';

export interface IEntityType extends IPurchasable {
    health: number;
    armor: number;
    mana: number;
}