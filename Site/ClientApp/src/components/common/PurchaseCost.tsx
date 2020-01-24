import React, { FunctionComponent } from 'react';
import { ReadonlyDictionary } from '../../data/Dictionary';
import './PurchaseCost.css';
import { EntityField } from './EntityField';

interface Props {
    cost: ReadonlyDictionary<string, number>;
}

export const PurchaseCost: FunctionComponent<Props> = props => {
    const resourceCosts: JSX.Element[] = [];

    for (const resource in props.cost) {
        const cost = props.cost[resource]!;
        resourceCosts.push(
            <EntityField
                className="purchaseCost__field"
                key={resource}
                name={resource}
                value={cost} 
            />);
    }

    if (resourceCosts.length === 0) {
        return null;
    }

    return (
        <div className="purchaseCost">
            <div className="purchaseCost__label">Cost</div>
            {resourceCosts}
        </div>
    )
}