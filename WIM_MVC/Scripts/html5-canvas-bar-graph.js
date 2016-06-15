// Copyright 2011 William Malone (www.williammalone.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

function BarGraph(ctx) {

    // Private properties and methods

    var that = this;
    var startArr;
    var endArr;
    var looping = false;


    // Loop method adjusts the height of bar and redraws if neccessary
    var loop = function () {

        var delta;
        var animationComplete = true;

        // Boolean to prevent update function from looping if already looping
        looping = true;

        // For each bar
        for (var i = 0; i < endArr.length; i += 1) {
            // Change the current bar height toward its target height
            delta = (endArr[i] - startArr[i]) / that.animationSteps;
            that.curArr[i] += delta;
            // If any change is made then flip a switch
            if (delta) {
                animationComplete = false;
            }
        }
        // If no change was made to any bars then we are done
        if (animationComplete) {
            looping = false;
        } else {
            // Draw and call loop again
            draw(that.curArr);
            setTimeout(loop, that.animationInterval / that.animationSteps);
        }
    };

    // Draw method updates the canvas with the current display
    var draw = function (arr) {

        var numOfBars = arr.length;
        var barWidth;
        var barHeight;
        var border = 0;
        var ratio;
        var maxBarHeight;
        var gradient;
        //var largestValue;
        var graphAreaX = 0;
        var graphAreaY = 0;
        var graphAreaWidth = that.width;
        var graphAreaHeight = that.height;
        var i;
        var x = 0;
        var x1 = 0;
        // Update the dimensions of the canvas only if they have changed
        if (ctx.canvas.width !== that.width || ctx.canvas.height !== that.height) {
            ctx.canvas.width = that.width;
            ctx.canvas.height = that.height;
        }
        graphAreaHeight -= 20;
        //draw graphBoder
        //ctx.fillStyle = "black";
        ctx.textAlign = "right";
        ctx.moveTo(30, 0);
        ctx.lineTo(30, graphAreaHeight);
        ctx.moveTo(30, 0);

        ctx.lineTo(graphAreaWidth, 0);
        ctx.moveTo(graphAreaWidth, graphAreaHeight);
        ctx.lineTo(graphAreaWidth, 0);
        ctx.moveTo(graphAreaWidth, graphAreaHeight);
        ctx.fillText("(kg)", 30, 10);
        ctx.fillText("(m)", graphAreaWidth, graphAreaHeight + 10);
        ctx.lineTo(30, graphAreaHeight);




        //var myb = 2;
        //ctx.fillStyle = "black";
        //ctx.fillRect(0 + myb, 0 + myb, that.width - myb * 2, graphAreaHeight - myb * 2);
        //ctx.fillRect(graphAreaHeight + myb, 0 + myb, that.width - myb * 2, graphAreaHeightt - myb * 2);

        // Draw the background color
        //ctx.fillStyle = that.backgroundColor;
        //ctx.fillRect(0, 0, that.width, that.height);

        // If x axis labels exist then make room	
        //if (that.xAxisLabelArr.length) {
        //graphAreaHeight -= 20;
        // }

        // Calculate dimensions of the bar
        //barWidth = graphAreaWidth / numOfBars - that.margin * 2;
        barWidth = 10;
        var headGraph = graphAreaHeight / 10;
        if (headGraph < 10) {
            headGraph = 15;
        } else {
            maxBarHeight = graphAreaHeight - headGraph;
        }


        // Determine the largest value in the bar array
        var largestValue = 0;
        for (i = 0; i < arr.length; i += 1) {
            if (arr[i] > largestValue) {
                largestValue = arr[i];
            }
        }
        if (that.maxVal) {
            that.maxValue = largestValue + 20;
        }
        var temp1 = (largestValue * 1.1 / 2).toFixed(0);
        temp1 = numberWithCommas(temp1);
        var temp2 = (((largestValue * 1.1) * 3) / 4).toFixed(0);
        temp2 = numberWithCommas(temp2);
        var temp3 = ((largestValue * 1.1) / 4).toFixed(0);
        temp3 = numberWithCommas(temp3);


        ctx.fillText(temp1, 25, ((graphAreaHeight - 25) / 2) / 0.8);
        ctx.moveTo(30, ((graphAreaHeight - 25) / 2) / 0.8);
        ctx.lineTo(graphAreaWidth, ((graphAreaHeight - 25) / 2) / 0.8);

        ctx.fillText(temp2, 25, ((graphAreaHeight - 25) / 4) / 0.8);
        ctx.moveTo(30, ((graphAreaHeight - 25) / 4) / 0.8);
        ctx.lineTo(graphAreaWidth, ((graphAreaHeight - 25) / 4) / 0.8)

        ctx.fillText(temp3, 25, ((graphAreaHeight - 25) * 3 / 4) / 0.8);
        ctx.moveTo(30, ((graphAreaHeight - 25) * 3 / 4) / 0.8);
        ctx.lineTo(graphAreaWidth, ((graphAreaHeight - 25) * 3 / 4) / 0.8)
        ctx.stroke();
        // For each bar
        for (i = 0; i < arr.length; i += 1) {
            if (arr[i] > 0) {
                // Set the ratio of current bar compared to the maximum
                if (that.maxValue) {
                    ratio = arr[i] / that.maxValue;
                } else {
                    ratio = arr[i] / (largestValue);
                    //ratio = arr[i] / graphAreaHeight;
                }

                barHeight = ratio * maxBarHeight;

                // Turn on shadow;
                ctx.shadowColor = "#999";

                // Draw bar background
                //ctx.fillStyle = "blue";
                ctx.fillStyle = that.colors[i];
                ctx.fillRect((i * that.width / numOfBars) * 80 / 100 + 20,
	                graphAreaHeight - barHeight,
	                barWidth,
	                barHeight);

                // Turn off shadow
                ctx.shadowOffsetX = 0;
                ctx.shadowOffsetY = 0;
                ctx.shadowBlur = 0;
                ctx.fillStyle = "#fff";
                // Draw bar color if it is large enough to be visible
                //if (arr[i] > 0) {
                //    if (barHeight > border * 2) {
                //        // Create gradient
                //        gradient = ctx.createLinearGradient(0, 0, 0, graphAreaHeight);
                //        gradient.addColorStop(1 - ratio, that.colors[i % that.colors.length]);
                //        gradient.addColorStop(1, "#ffffff");

                //ctx.fillStyle = "blue";
                //        // Fill rectangle with gradient
                //ctx.fillRect(that.margin + i * that.width / numOfBars + border,
                //    graphAreaHeight - barHeight + border,
                //    barWidth - border * 2,
                //    barHeight - border * 2);
                //}
                //}

                // Write bar value
                //ctx.fillStyle = "#333";
                ctx.fillStyle = "black";
                ctx.font = "bold 10px sans-serif";
                ctx.textAlign = "left";
                // Use try / catch to stop IE 8 from going to error town
                //if (arr[i] > 0) {
                try {
                    
                    ctx.fillText(numberWithCommas(arr[i]),
                        (i * that.width / numOfBars) * 80 / 100 + 20,
                        graphAreaHeight - barHeight - 3);
                } catch (ex) {
                }
                //}
                // Draw bar label if it exists
                //if (that.xAxisLabelArr[i]) {					
                //  // Use try / catch to stop IE 8 from going to error town				
                //  ctx.fillStyle = "#333";
                //  ctx.font = "bold 12px sans-serif";
                //  ctx.textAlign = "center";
                //  try{
                //	ctx.fillText(that.xAxisLabelArr[i],
                //	  i * that.width / numOfBars + (that.width / numOfBars) / 2,
                //	  that.height - 10);
                //	} catch (ex) {}
                //  }

                //if (x > 0) {
                var gap = i - 100;
                if (gap > 0) {
                    var id = (i - x) / 100;
                    var xv = (i + x) / 2;
                    ctx.fillStyle = "#000";
                    ctx.font = "bold 10px sans-serif";
                    ctx.textAlign = "center";
                    try {
                        
                        ctx.fillText(id,
	                      (xv * that.width / numOfBars + (that.width / numOfBars) / 2) * 80 / 100 + 20,
	                      that.height - 5);
                        //ctx.fillText(id,
                        //    i * that.width / numOfBars + (that.width / numOfBars) / 2 - 25,
                        //    that.height - 5);
                    } catch (ex) {
                    }


                }
                x = i;



            }

            //for (i = 0; i < distances.length - 1; i += 1) {
            //    var id = (parseFloat(distances[i]) + parseFloat(distances[i + 1])) * 100;
            //          ctx.fillStyle = "#333";
            //          ctx.font = "bold 10px sans-serif";
            //          ctx.textAlign = "center";
            //          try{
            //        	ctx.fillText(id,
            //        	  id/2,
            //        	  that.height - 5);
            //        	} catch (ex) {}
            //}
        }
    };

    // Public properties and methods

    this.width = 300;
    this.height = 150;
    this.maxValue;
    this.margin = 5;
    this.colors = ["purple", "red", "green", "yellow"];
    this.curArr = [];
    this.backgroundColor = "#fff";
    this.xAxisLabelArr = [];
    this.yAxisLabelArr = [];
    this.animationInterval = 100;
    this.animationSteps = 10;
    this.xvalues = [];
    this.baseDistance = 0;
    this.distances = [];
    // Update method sets the end bar array and starts the animation
    this.update = function (newArr) {

        // If length of target and current array is different 
        //if (that.curArr.length !== newArr.length) {
        that.curArr = newArr;
        draw(newArr);
        //} else {
        //  // Set the starting array to the current array
        //  startArr = that.curArr;
        //  // Set the target array to the new array
        //  endArr = newArr;
        //  // Animate from the start array to the end array
        //  if (!looping) {	
        //    loop();
        //  }
        //}
    };

    function numberWithCommas(x) {
        var parts = x.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    };

    this.drawDistance = function (ctx1, distances) {
        ctx1.fillStyle = "blue";
        ctx1.font = "bold 20px sans-serif";
        ctx1.textAlign = "center";
        var text = "doanh";
        // Use try / catch to stop IE 8 from going to error town
        var i = 0;
        for (i = 0; i < values.length - 1; i += 1) {
            //if (distances[i] > 0) {
            var x = distances[i] * 100 + baseDistance;
            var x1 = distances[i + 1] * 100 + baseDistance;
            var xm = (x + x1) / 2;

            try {
                ctx1.strokeText(//parseFloat(distances[i]).toFixed(2),
                    text,
                    xm,
                    5);
            } catch (ex) {
            }
            //}
        }
    }
}