let state = { IsPlaying: false }

// NOT NORMAL QUEUE ONLY FOR UPDATE PURPOSES
var queue = []

const processQueue = () => {
    if (queue.length > 0) {
        while (queue.length > 0) {
            const { data } = queue.pop();

            updateState(data);
        }
    }
}

const queueUpdate = (data) => {
    queue.push({ data });
}

const processPayload = payload => {
    const { eventName, data } = payload

    switch (eventName) {
        case "player-status": {
            updateState(data);

            break;
        }
    }
}

const processUpdate = (data) => {
    queueUpdate(data);
}

const organizeArtists = (artists, useUrl=false) => {
  var str = ""

  for(let i = 0; i < artists.length; i++) {
    var artist = artists[i]

    if(useUrl)
      str += `<a href="${artist.url}" target="_blank">${artist.name}</a>`
    else str += artist.name

    if(i < artists.length - 1) str += ", "
  }

  return str
}

const getTime = time => {
  return `${Math.floor(time / 60)}:${(time % 60).toLocaleString("en-US", { minimumIntegerDigits: 2, minimumFractionDigits: 0 })}`
}

function updateState(data) {
    var { position } = state

    position += 1

    state = { ...data, position }

    console.log(state)

    if(state.IsPlaying) {
        $("#artwork").attr("src", state.song.artwork_url)
        $("#title").html(state.song.title)
        $("#artists").html(organizeArtists(state.song.artists))
        $("#album").html(state.song.album)
        $("#total-time").html(getTime(state.song.duration))
        $("#pos").css("width", `${parseInt($("#slider").width() - 4) * (state.song.position / state.song.duration)}px`)
        $("#current-time").html(getTime(state.song.position))
    } else {
        $("#artwork").attr("src", "/static/images/blank-album-artwork.png")
        $("#title").html("Not Playing")
        $("#artists").html("N/A")
        $("#album").html("N/A")
        $("#total-time").html(getTime(0))
        $("#pos").css("width", `0px`)
        $("#current-time").html(getTime(0))
    }
}

var timeSinceLastCheck = 0

const continueState = () => {
    processQueue();

    if (state.IsPlaying) {
        if(state.song.position < state.song.duration) {
          $("#pos").css("width", `${(parseInt($("#slider").width()) - 4) * (state.song.position / state.song.duration)}px`)
          $("#current-time").html(getTime(state.song.position))
          state.song.position += 1
        } else if(state.song.position == state.song.duration) {
          // pullData()
        }
    } else if (!state.IsPlaying && timeSinceLastCheck < 60) {
    timeSinceLastCheck++
    } else if (!state.IsPlaying && timeSinceLastCheck >= 60) {
    timeSinceLastCheck = 0
    // pullData()
  }
}

// Update slider
setInterval(() => {
    continueState();
}, 1000);

var ws = null;

const init = () => {
    ws = new WebSocket("wss://" + location.host + "/ws")

    const SUBSCRIPTIONS = ['update:player-status:1'];

    ws.addEventListener('open', (_, ev) => {
        ws.send(JSON.stringify({ event: "player-status", data: 1 }));

        SUBSCRIPTIONS.forEach(v => ws.send(JSON.stringify({ event: "subscribe", data: v })));
    });

    ws.addEventListener('message', (e) => {
        console.log(e);

        const { event, data } = JSON.parse(e.data);

        switch (event) {
            case "message": {
                console.log(`[WebSocket Response] "${data}"`);
                break;
            }
            case "data": {
                processPayload(data);
                break;
            }
            default: {
                if (type.includes("update")) {
                    processUpdate(data);
                } else {
                    console.log({ event, data })
                }
            }
        }
    });
}

setTimeout(() => init(), 50);