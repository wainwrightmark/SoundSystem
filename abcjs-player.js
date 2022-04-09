// This object is the class that will contain the buffer
var midiBuffer;
var visualObj;
var cursor;
var timingCallbacks;

var lastEls = [];
function colorElements(els) {
    var i;
    var j;
    for (i = 0; i < lastEls.length; i++) {
        for (j = 0; j < lastEls[i].length; j++) {
            lastEls[i][j].classList.remove("color");
        }
    }
    for (i = 0; i < els.length; i++) {
        for (j = 0; j < els[i].length; j++) {
            els[i][j].classList.add("color");
        }
    }
    lastEls = els;
}

function eventCallback(ev) {
    if (!ev) {
        return;
    }
    colorElements(ev.elements);
}


function beatCallback(currentBeat,totalBeats,lastMoment,position, debugInfo) {
    var x1, x2, y1, y2;
    if (currentBeat === totalBeats) {
        x1 = 0;
        x2 = 0;
        y1 = 0;
        y2 = 0;
    } else {
        x1 = position.left - 2;
        x2 = position.left - 2;
        y1 = position.top;
        y2 = position.top + position.height;
    }
    cursor.setAttribute("x1", x1);
    cursor.setAttribute("x2", x2);
    cursor.setAttribute("y1", y1);
    cursor.setAttribute("y2", y2);
    colorElements([]);
        
}

function createCursor() {
    var svg = document.querySelector("#paper svg");
    var cursor = document.createElementNS("http://www.w3.org/2000/svg", "line");
    cursor.setAttribute("class", "abcjs-cursor");
    cursor.setAttributeNS(null, 'x1', 0);
    cursor.setAttributeNS(null, 'y1', 0);
    cursor.setAttributeNS(null, 'x2', 0);
    cursor.setAttributeNS(null, 'y2', 0);
    svg.appendChild(cursor);
    return cursor;
}


function load(abcData) {
    stop();
    // First draw the music - this supplies an object that has a lot of information about how to create the synth.
    // NOTE: If you want just the sound without showing the music, use "*" instead of "paper" in the renderAbc call.
    visualObj = ABCJS.renderAbc("paper",
        abcData,
        {
            responsive: "resize"
        })[0];
    cursor = createCursor();

    timingCallbacks= new ABCJS.TimingCallbacks(visualObj, {
        beatCallback: beatCallback,
        eventCallback: eventCallback
    });
}


function stop() {
    if (midiBuffer)
        midiBuffer.stop();

    if (timingCallbacks)
        timingCallbacks.stop();
}


function play() {

    stop();

    if (ABCJS.synth.supportsAudio()) {

        // An audio context is needed - this can be passed in for two reasons:
        // 1) So that you can share this audio context with other elements on your page.
        // 2) So that you can create it during a user interaction so that the browser doesn't block the sound.
        // Setting this is optional - if you don't set an audioContext, then abcjs will create one.
        window.AudioContext = window.AudioContext ||
            window.webkitAudioContext ||
            navigator.mozAudioContext ||
            navigator.msAudioContext;
        var audioContext = new window.AudioContext();
        audioContext.resume().then(function() {
            // In theory the AC shouldn't start suspended because it is being initialized in a click handler, but iOS seems to anyway.

            // This does a bare minimum so this object could be created in advance, or whenever convenient.
            midiBuffer = new ABCJS.synth.CreateSynth();

            // midiBuffer.init preloads and caches all the notes needed. There may be significant network traffic here.
            return midiBuffer.init({
                visualObj: visualObj,
                options: {
                chordsOff: true,
            },
                
                audioContext: audioContext,
                millisecondsPerMeasure: visualObj.millisecondsPerMeasure()
            }).then(function(response) {
                console.log("Notes loaded: ", response);
                // console.log(response); // this contains the list of notes that were loaded.
                // midiBuffer.prime actually builds the output buffer.
                return midiBuffer.prime();
            }).then(function(response) {
                // At this point, everything slow has happened. midiBuffer.start will return very quickly and will start playing very quickly without lag.
                timingCallbacks.start();
                midiBuffer.start();
                return Promise.resolve();
            }).catch(function(error) {
                if (error.status === "NotSupported") {
                    console.warn("Not Supported", error);
                } else
                    console.warn("synth error", error);
            });
        });
    }
}

