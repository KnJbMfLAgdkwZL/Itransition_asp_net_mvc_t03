// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function GetAddressees() {
    if (this.value.length >= 3) {

        /*fetch("/Chat/GetAddressees?" + (new URLSearchParams({
            search: this.value
        })).toString(), {
            method: "GET"
        }).then((response) => {
            return response.json();
        }).then((data) => {
            if (data.length > 0) {
                let cord = getCaretGlobalCoordinates(this)
                addresRecomSetCord(cord)
                addresRecomShow(data)
            }
        })*/


        let data = JSON.stringify({
            Type: 'GetUsersEmail',
            Data: {
                Search: this.value
            }
        })
        sendMessage(data)


    }
}

function addresRecomHide() {
    addresRecom.innerHTML = ''
    addresRecom.style.display = 'none'
}

function addresRecomSetCord(cord) {
    addresRecom.style.top = cord.top
    addresRecom.style.left = cord.left
}

function addresRecomShow(data) {
    let str = ''
    data.forEach(v => str += `<div class="addressesRecommendations" onMouseDown="addressesRecommendationsClick(this)">${v}</div>`)
    addresRecom.innerHTML = str
    addresRecom.style.display = 'inline-block'
}

addressee.oninput = GetAddressees
addressee.onfocus = GetAddressees

function addressesRecommendationsClick(el) {
    addressee.value = el.innerHTML
    console.log(el.innerHTML)

    addresRecomHide()
}

addressee.onblur = function () {
    addresRecomHide()
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

function getCaretGlobalCoordinates(Desired_ID) {
    let coordinates = getCaretCoordinates(Desired_ID, Desired_ID.selectionStart);
    let elementCoordinates = getElementCoords(Desired_ID);
    let top = coordinates.top + elementCoordinates.top;
    let left = coordinates.left + elementCoordinates.left;
    return {top: `${top}px`, left: `${left}px`};
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


send.onclick = function () {
    let data = JSON.stringify({
        Type: 'CreateMessage',
        Data: {
            Addressee: addressee.value,
            Head: head.value,
            Body: body.value
        }
    })
    sendMessage(data)
}


let scheme = document.location.protocol === "https:" ? "wss" : "ws";
let port = document.location.port ? (":" + document.location.port) : "";
let connectionUrl = scheme + "://" + document.location.hostname + port + "/ws";
let socket = new WebSocket(connectionUrl);


function closeSocket() {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        console.log("socket not connected");
    }
    socket.close(1000, "Closing from client");
}

function sendMessage(data) {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        console.log("socket not connected");
    }
    socket.send(data);
}

socket.onopen = function (event) {
    console.log(`onopen`)
}
socket.onclose = function (event) {
    console.log(`onclose: ${event.code}`)
}
socket.onmessage = function (event) {
    console.log(`onmessage: ${event.data}`)
    let response = JSON.parse(event.data)
    if (response && response.Type) {
        let caseMethod = WebSocketResponseCases[response.Type]
        if (caseMethod) {
            caseMethod(response)
        }
    }
}
socket.onerror = function () {
    if (socket) {
        switch (socket.readyState) {
            case WebSocket.CLOSED:
                console.log(`onerror: WebSocket.CLOSED`)
                break;
            case WebSocket.CLOSING:
                console.log(`onerror: WebSocket.CLOSING`)
                break;
            case WebSocket.CONNECTING:
                console.log(`onerror: WebSocket.CONNECTING`)
                break;
            case WebSocket.OPEN:
                console.log(`onerror: WebSocket.OPEN`)
                break;
            default:
                console.log(`Unknown WebSocket State: ${socket.readyState}`)
                break;
        }
    }
}


let WebSocketResponseCases = {
    GetUsersEmailResponse: function (response) {
        if (response.Data) {
            let data = response.Data
            if (data.length > 0) {
                let cord = getCaretGlobalCoordinates(addressee)
                addresRecomSetCord(cord)
                addresRecomShow(data)
            }
        }
    },
    CreateMessageResponse: function (response) {
        console.log('CreateMessageResponse ok')
        /*if (response.data) {
            let data = response.data
            if (data.length > 0) {
                let cord = getCaretGlobalCoordinates(addressee)
                addresRecomSetCord(cord)
                addresRecomShow(data)
            }*/
    }
}

