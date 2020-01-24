import React, { FunctionComponent } from 'react';
import { IEntityType } from '../../data/definitions/IEntityType';
import { EntityField } from './EntityField';
import { PurchaseCost } from './PurchaseCost';
import './EntityDetails.css';

interface Props {
    entity: IEntityType;
    prerequisite?: IEntityType;
}

export const EntityDetails: FunctionComponent<Props> = props => {
    const prerequisite = props.prerequisite
        ? <div className="entityDetails__prerequisite">Requires <span className="entityDetails__value">{props.prerequisite.symbol}</span></div>
        : undefined; // ought to use name here, but they're all the same currently
    
    const cost = props.entity.cost
        ? <PurchaseCost cost={props.entity.cost} />
        : undefined;

    return (
        <div className="entityDetails">
            <div className="entityDetails__name">{props.entity.name}</div>
            {prerequisite}
            {cost}
            <EntityField className="entityField--health" name="Health" value={props.entity.health} />
            <EntityField className="entityField--armor" name="Armor" value={props.entity.armor} />
            <EntityField className="entityField--mana" name="Mana" value={props.entity.mana} />
        </div>
    );
}