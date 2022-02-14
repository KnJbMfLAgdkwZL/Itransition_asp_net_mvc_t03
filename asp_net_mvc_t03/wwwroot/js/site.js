// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

addressee.oninput = function () {
    if (this.value.length >= 3) {
        fetch("/Chat/GetAddressees?" + (new URLSearchParams({
            search: this.value
        })).toString(), {
            method: "GET"
        }).then((response) => {
            return response.json();
        }).then((data) => {
            myFunction(this)
            addressees.innerHTML = data.join('<br/>')
            addressees.style.display = 'inline-block'
        })
    }
}

function getElementCoords(elem) {
    let box = elem.getBoundingClientRect();
    let body = document.body;
    let docEl = document.documentElement;
    let scrollTop = window.pageYOffset || docEl.scrollTop || body.scrollTop;
    let scrollLeft = window.pageXOffset || docEl.scrollLeft || body.scrollLeft;
    let clientTop = docEl.clientTop || body.clientTop || 0;
    let clientLeft = docEl.clientLeft || body.clientLeft || 0;
    let top = box.top + scrollTop - clientTop;
    let left = box.left + scrollLeft - clientLeft;
    return {top: Math.round(top), left: Math.round(left)};
}

function myFunction(Desired_ID) {
    //document.getElementById(Desired_ID.id).style.backgroundColor = "red";
    let coordinates = getCaretCoordinates(Desired_ID, Desired_ID.selectionStart);
    let elementCoordinates = getElementCoords(Desired_ID);
    let top = coordinates.top + elementCoordinates.top;
    let left = coordinates.left + elementCoordinates.left;

    addressees.style.top = `${top}px`
    addressees.style.left = `${left}px`
}

(function () {
    let properties = ['direction', 'boxSizing', 'width', 'height', 'overflowX', 'overflowY', 'borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth', 'borderStyle', 'paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft', 'fontStyle', 'fontVariant', 'fontWeight', 'fontStretch', 'fontSize', 'fontSizeAdjust', 'lineHeight', 'fontFamily', 'textAlign', 'textTransform', 'textIndent', 'textDecoration', 'letterSpacing', 'wordSpacing', 'tabSize', 'MozTabSize'];
    let isBrowser = (typeof window !== 'undefined');
    let isFirefox = (isBrowser && window.mozInnerScreenX != null);

    function getCaretCoordinates(element, position, options) {
        if (!isBrowser) {
            throw new Error('textarea-caret-position#getCaretCoordinates should only be called in a browser');
        }
        let debug = options && options.debug || false;
        if (debug) {
            let el = document.querySelector('#input-textarea-caret-position-mirror-div');
            if (el) {
                el.parentNode.removeChild(el);
            }
        }
        let div = document.createElement('div');
        div.id = 'input-textarea-caret-position-mirror-div';
        document.body.appendChild(div);
        let style = div.style;
        let computed = window.getComputedStyle ? getComputedStyle(element) : element.currentStyle;
        style.whiteSpace = 'pre-wrap';
        if (element.nodeName !== 'INPUT') style.wordWrap = 'break-word';
        style.position = 'absolute';
        if (!debug) style.visibility = 'hidden';
        properties.forEach(function (prop) {
            style[prop] = computed[prop];
        });
        if (isFirefox) {
            if (element.scrollHeight > parseInt(computed.height)) style.overflowY = 'scroll';
        } else {
            style.overflow = 'hidden';
        }
        div.textContent = element.value.substring(0, position);
        if (element.nodeName === 'INPUT') div.textContent = div.textContent.replace(/\s/g, '\u00a0');
        let span = document.createElement('span');
        span.textContent = element.value.substring(position) || '.';
        div.appendChild(span);
        let coordinates = {
            top: span.offsetTop + parseInt(computed['borderTopWidth']),
            left: span.offsetLeft + parseInt(computed['borderLeftWidth'])
        };
        if (debug) {
            span.style.backgroundColor = '#aaa';
        } else {
            document.body.removeChild(div);
        }
        return coordinates;
    }

    if (typeof module != 'undefined' && typeof module.exports != 'undefined') {
        module.exports = getCaretCoordinates;
    } else if (isBrowser) {
        window.getCaretCoordinates = getCaretCoordinates;
    }
}());
