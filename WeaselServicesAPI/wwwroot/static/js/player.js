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
      str += `<a href="${artist.Uri}" target="_blank">${artist.Name}</a>`
    else str += artist.Name

    if(i < artists.length - 1) str += ", "
  }

  return str
}

const getTime = time => {
    return `${Math.floor(time / 60)}:${Math.floor(time % 60).toLocaleString("en-US", { minimumIntegerDigits: 2, minimumFractionDigits: 0 })}`
}

function updateState(data) {
    var { position } = state

    position += 1

    state = { ...data, position }

    console.log(state)

    if (state.IsPlaying) {
        var duration = state.CurrentSong.DurationMs / 1000;
        var songPos = state.ProgressMs / 1000;

        $("#artwork").attr("src", state.CurrentSong.Album.ArtworkURL)
        $("#title").html(state.CurrentSong.Name)
        $("#artists").html(organizeArtists(state.CurrentSong.Artists))
        $("#album").html(state.CurrentSong.Album.Name)
        $("#total-time").html(getTime(state.CurrentSong.DurationMs / 1000))
        $("#pos").css("width", `${parseInt($("#slider").width() - 4) * (songPos / duration)}px`)
        $("#current-time").html(getTime(songPos))
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
        var duration = state.CurrentSong.DurationMs / 1000;
        var songPos = state.ProgressMs / 1000;

        if (songPos < duration) {
            $("#pos").css("width", `${(parseInt($("#slider").width()) - 4) * (songPos / duration)}px`)
            $("#current-time").html(getTime(songPos))
            state.ProgressMs += 1000;
        } else if (songPos >= duration) {
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
    // ws = new WebSocket("wss://" + location.host + "/ws")
    ws = new WebSocket("wss://qa.api.noahtemplet.dev/ws")

    const SUBSCRIPTIONS = ['update:player-status:1'];

    ws.addEventListener('open', (_, ev) => {
        ws.send(JSON.stringify({ event: "player-status", data: 1 }));

        SUBSCRIPTIONS.forEach(v => ws.send(JSON.stringify({ event: "subscribe", data: v })));
    });

    ws.addEventListener('message', (e) => {
        const { event, data } = JSON.parse(e.data);

        console.log({ event, data })

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
                if (event.includes("update")) {
                    processUpdate(data);
                } else {
                    console.log({ event, data })
                }
            }
        }
    });
}

setTimeout(() => init(), 50);