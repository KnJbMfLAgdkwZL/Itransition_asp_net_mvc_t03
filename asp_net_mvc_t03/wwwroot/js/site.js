// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
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

function HideAddressRecommendation() {
    let addressRecommendation = document.getElementById('addressRecommendation')
    addressRecommendation.innerHTML = ''
    addressRecommendation.style.display = 'none'
}

function SetAddressRecommendationCoordinates(cord) {
    let addressRecommendation = document.getElementById('addressRecommendation')
    addressRecommendation.style.top = cord.top
    addressRecommendation.style.left = cord.left
}

function ShowAddressRecommendation(data) {
    let addressRecommendation = document.getElementById('addressRecommendation')
    let str = ''
    data.forEach(v => str += `<div class="addressesRecommendations">${v}</div>`)
    addressRecommendation.innerHTML = str
    addressRecommendation.style.display = 'inline-block'
    for (let v of document.getElementsByClassName('addressesRecommendations')) {
        v.onmousedown = addressesRecommendationsClick
    }
}

function addressesRecommendationsClick() {
    let toUser = document.getElementById('toUser')
    toUser.value = this.innerHTML
    HideAddressRecommendation()
}

function FillTopics(data) {

    let url_str = window.location.href
    let url = new URL(url_str);
    let search_params = url.searchParams;
    let id = null
    let head = null
    if (search_params.has('id') && search_params.has('head')) {
        id = search_params.get('id')
        head = search_params.get('head')
    }

    let str = ''
    for (let v of data) {
        let date = new Date(v.CreateDate)
        let dateStr = date.toLocaleTimeString() + ' ' + date.toLocaleDateString()
        let active_chat = ''
        if (id === v.Id || head === v.Head) {
            active_chat = 'active_chat'
        }
        str += `
        <a href="/Chat/Dialog?id=${v.Id}&head=${v.Head}" class="aTopics">
            <div class="chat_list ${active_chat}" messageId="${v.Id}">
                <div class="chat_people">
                    <div class="chat_ib">
                        <h5>${v.Author.Name} (${v.Author.Email}) <span class="chat_date">${dateStr}</span></h5>
                        <p>
                            <b>
                                ${v.Head}
                            </b>
                        </p>
                        <p>
                            ${v.Body.slice(0, 120)}
                        </p>
                    </div>
                </div>
            </div>
        </a>`
    }
    document.getElementsByClassName('inbox_chat')[0].innerHTML = str
}

function FillDialog(data) {
    let userId = data.UserId
    let str = ``
    for (let v of data.Messages) {
        let date = new Date(v.CreateDate)
        let dateStr = date.toLocaleTimeString() + ' ' + date.toLocaleDateString()
        if (v.AuthorId === userId) {
            str +=
                `<div class="incoming_msg">
                    <div class="received_msg received_withd_msg">
                        ${v.Author.Name} (${v.Author.Email})
                        <p>
                            ${v.Body}
                        </p>
                        <span class="time_date">${dateStr}</span>
                    </div>
                </div>`
        } else {
            str +=
                `<div class="outgoing_msg">
                    <div class="sent_msg">
                        ${v.Author.Name} (${v.Author.Email})
                        <p>
                            ${v.Body}
                        </p>
                        <span class="time_date">${dateStr}</span>
                    </div>
                </div>`
        }
    }
    document.getElementsByClassName('msg_history')[0].innerHTML = str
}

class WebSocketWrapper {
    constructor() {
        let scheme = document.location.protocol === "https:" ? "wss" : "ws";
        let port = document.location.port ? (":" + document.location.port) : "";
        let connectionUrl = scheme + "://" + document.location.hostname + port + "/ws";
        this.socket = new WebSocket(connectionUrl);
        this.socket.onopen = this.onopen
        this.socket.onclose = this.onclose
        this.socket.onmessage = this.onmessage
        this.socket.onerror = this.onerror
    }

    closeSocket() {
        let socket = this.socket

        if (!socket || socket.readyState !== WebSocket.OPEN) {
            console.log("socket not connected");
        }
        socket.close(1000, "Closing from client");
    }

    sendMessage(data) {
        let socket = this.socket

        if (!socket) {
            console.log(`socket is false socket: ${socket}`)
            return
        }
        if (socket.readyState !== WebSocket.OPEN) {
            console.log(`socket not connected State: ${socket.readyState}`);
        }
        if (socket.readyState === WebSocket.CONNECTING) {
            setTimeout(() => socket.send(data), 500)
        }
        if (socket.readyState === WebSocket.OPEN) {
            socket.send(data);
        }
    }

    onopen(event) {
        console.log(`onopen`)
    }

    onclose(event) {
        console.log(`onclose: ${event.code}`)
    }

    onmessage(event) {
        console.log(`onmessage: ${event.data}`)
        let response = JSON.parse(event.data)
        if (response && response.Type) {
            let caseMethod = WebSocketResponseCases[response.Type]
            if (caseMethod) {
                caseMethod(response)
            }
        }
    }

    onerror() {
        let socket = this.socket

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
}

let WebSocketResponseCases = {
    GetUsersEmailResponse: function (response) {
        if (!response.Error) {
            if (response.Data) {
                let data = response.Data
                if (data.length > 0) {
                    let toUser = document.getElementById('toUser')
                    let cord = getCaretGlobalCoordinates(toUser)
                    SetAddressRecommendationCoordinates(cord)
                    ShowAddressRecommendation(data)
                }
            }
        }
    },
    CreateMessageResponse: function (response) {
        if (!response.Error) {
            if (response.Data) {
                document.location.href = `/Chat/Dialog?id=${response.Data.Id}&head=${response.Data.Head}`
            }
        }
    },
    GetTopicsResponse: function (response) {
        if (!response.Error) {
            if (response.Data) {
                FillTopics(response.Data)
            }
        }
    },
    GetMessagesResponse: function (response) {
        if (!response.Error) {
            if (response.Data) {
                FillDialog(response.Data)
            }
        }
    }
}

if (window.location.href.indexOf('Chat/Index') > -1) {
    console.log('Chat/Index')
    let webSocketWrapper = new WebSocketWrapper()

    let toUser = document.getElementById('toUser')
    toUser.oninput = toUser.onfocus = function () {
        if (this.value.length >= 3) {
            webSocketWrapper.sendMessage(JSON.stringify({
                Type: 'GetUsersEmail', Data: {
                    Search: this.value
                }
            }))
        }
    }
    toUser.onblur = function () {
        HideAddressRecommendation()
    }

    let head = document.getElementById('head')
    let body = document.getElementById('body')

    document.getElementById('send').onclick = function () {
        webSocketWrapper.sendMessage(JSON.stringify({
            Type: 'CreateMessage', Data: {
                ToUser: toUser.value, Head: head.value, Body: body.value
            }
        }))
    }

    webSocketWrapper.sendMessage(JSON.stringify({
        Type: 'GetTopics', Data: null
    }))
    
}

if (window.location.href.indexOf('Chat/Dialog') > -1) {
    console.log('Chat/Dialog')
    let webSocketWrapper = new WebSocketWrapper()

    let url_str = window.location.href
    let url = new URL(url_str);
    let search_params = url.searchParams;

    if (search_params.has('id')) {

        webSocketWrapper.sendMessage(JSON.stringify({
            Type: 'GetMessages', Data: {
                Id: search_params.get('id')
            }
        }))

        webSocketWrapper.sendMessage(JSON.stringify({
            Type: 'GetTopics', Data: null
        }))


    }


}
