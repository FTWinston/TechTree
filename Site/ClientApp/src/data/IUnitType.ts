import { IPurchasable } from './IPurchaseable';

export interface IUnitType extends IPurchasable {
    builtBy: number;
}