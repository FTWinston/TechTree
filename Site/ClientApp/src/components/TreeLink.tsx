import React, { FunctionComponent } from 'react';

interface Props {
    className?: string;
    left: boolean;
    right: boolean;
    top: boolean;
    bottom: boolean;
}

export const TreeLink: FunctionComponent<Props> = props => {
    const className = props.className === undefined
        ? 'treeLink'
        : 'treeLink ' + props.className;

    const content = props.top
        ? props.bottom
            ? props.left
                ? props.right
                    ? '╬'
                    : '╣'
                : props.right
                    ? '╠'
                    : '║'
            : props.left
                ? props.right
                    ? '╩'
                    : '╝'
                : props.right
                    ? '╚'
                    : '╨'
        : props.bottom
            ? props.left
                ? props.right
                    ? '╦'
                    : '╗'
                : props.right
                    ? '╔'
                    : '╥'
            : props.left
                ? props.right
                    ? '═'
                    : '╡'
                : props.right
                    ? '╞'
                    : '' // none

    return (
        <div className={className}>
            {content}
        </div>
    )
}