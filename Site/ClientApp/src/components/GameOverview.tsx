import React, { FunctionComponent, useState } from 'react';
import { IGame } from '../data/instances/IGame';
import { Battlefield } from './Battlefield';
import { TechTree } from './tree';
import './GameOverview.css';

interface Props {
    data: IGame;
}

enum Element {
    TechTree,
    Battlefield,
}

export const GameOverview: FunctionComponent<Props> = props => {
    const [focus, setFocus] = useState<Element>();
    
    const classes = focus === undefined
        ? 'gameOverview'
        : 'gameOverview gameOverview--focused';

    return (
        <div className={classes}>
            {renderOverviewElement(focus, setFocus, Element.TechTree, <TechTree data={props.data.techTree} />)}
            {renderOverviewElement(focus, setFocus, Element.Battlefield, <Battlefield data={props.data.battlefield} />)}
        </div>
    );
}

function renderOverviewElement(
    focus: Element | undefined,
    setFocus: (element: Element | undefined) => void,
    element: Element,
    content: JSX.Element
) {
    const classes = focus === element
        ? 'gameOverview__element gameOverview__element--focus'
        : 'gameOverview__element';

    return (
        <div
            className={classes}
            onClick={() => setFocus(focus === element ? undefined : element)}
        >
            {content}
        </div>
    );
}