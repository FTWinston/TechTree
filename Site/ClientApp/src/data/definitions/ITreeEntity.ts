import { IPurchasable } from './IPurchaseable';

export interface ITreeEntity extends IPurchasable {
    health: number;
    mana: number;
    armor: number;
}