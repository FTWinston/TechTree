import React, { FunctionComponent, useMemo } from 'react';
import { Tooltip } from './Tooltip';
import './Icon.css';

export enum IconStyle {
    Normal = 0,
    Disabled = 1,
    Prerequisite = 2,
}

interface Props {
    symbol: string;
    name: string;
    className?: string;
    style?: IconStyle;
}

export const Icon: FunctionComponent<Props> = props => {
    const rootClasses = props.className === undefined
        ? 'icon'
        : 'icon ' + props.className;

    const iconClasses = useMemo(
        () => {
            let className = 'icon__icon';

            if (props.style !== undefined) {
                if (props.style & IconStyle.Disabled) {
                    className += ' icon__icon--disabled';
                }
                if (props.style & IconStyle.Prerequisite) {
                    className += ' icon__icon--prerequisite';
                }
            }

            return className;
        },
        [props.style]
    );

    const tooltip = props.children
        ? <Tooltip className="icon__tooltip">{props.children}</Tooltip>
        : undefined;

    return (
        <div className={rootClasses}>
            <div className={iconClasses} title={props.name}>
                {props.symbol}
            </div>
            {tooltip}
        </div>
    );
}