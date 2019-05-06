function calculate() {
  var textareaElement = document.getElementById('data-input');
  var resultElement = document.getElementById('result');
  var parsed = parseInput(textareaElement.value);
  if (!parsed) {
    resultElement.innerHTML = 'Error parsing input';
    return;
  }
  var result = doCalculation(parsed);

  resultElement.innerHTML = `Result is ${result}`;
}

function parseInput(input) {
  var result = [];
  input.split('\n').forEach(x => {
    if (x !== '') {
      var el = [];
      x.split(',').forEach(y => {
        if (y !== '') {
          el.push(Number(y));
        }
      });
      result.push({ x: el[0], y: el[1] });
    }
  });
  if (result.every(x => x && !Number.isNaN(x.x) && !Number.isNaN(x.y))) {
    return result;
  } else {
    return undefined;
  }
}

function doCalculation(input) {
  var indicesSortedByHeight = input
    .slice(0)
    .sort((a, b) => b.y - a.y)
    .map(x => input.findIndex(y => y === x));
  var pointNextToHighestIndex =
    indicesSortedByHeight[0] === input.length - 1
      ? indicesSortedByHeight[0] - 1
      : indicesSortedByHeight[0] + 1;
  var intersection = findIntersection(
    input[indicesSortedByHeight[0]],
    input[pointNextToHighestIndex],
    input[indicesSortedByHeight[1]],
    {
      x: input[indicesSortedByHeight[0]].x,
      y: input[indicesSortedByHeight[1]].y
    }
  );

  var intersection2 = findIntersection(
    input[indicesSortedByHeight[0]],
    input[pointNextToHighestIndex],
    input[indicesSortedByHeight[2]],
    {
      x: input[indicesSortedByHeight[0]].x,
      y: input[indicesSortedByHeight[2]].y
    }
  );

  var area = trapezoidArea(
    intersection,
    input[indicesSortedByHeight[1]],
    input[indicesSortedByHeight[2]],
    intersection2
  );

  return area;
}

function findIntersection(A, B, C, D) {
  var a1 = B.y - A.y;
  var b1 = A.x - B.x;
  var c1 = a1 * A.x + b1 * A.y;

  var a2 = D.y - C.y;
  var b2 = C.x - D.x;
  var c2 = a2 * C.x + b2 * C.y;

  var determinant = a1 * b2 - a2 * b1;

  if (determinant == 0) {
    // The lines are parallel.
    return undefined;
  } else {
    var x = (b2 * c1 - b1 * c2) / determinant;
    var y = (a1 * c2 - a2 * c1) / determinant;
    return { x: x, y: y };
  }
}

function trapezoidArea(A, B, C, D) {
  if (A.y !== B.y || C.y !== D.y) {
    return undefined;
  }
  var height = Math.abs(A.y - C.y);
  return (Math.abs(A.x - B.x) + Math.abs(C.x - D.x) / 2) * height;
}
