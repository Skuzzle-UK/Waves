<?php
header("Access-Control-Allow-Origin: *");
$servername = "obscured";
$username = "obscured";
$password = "obscured";
$dbname = "obscured";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Sanitize and validate inputs
$bestscore = intval($_GET['bestscore']);
$name = trim($_GET['name']);
$limit = intval($_GET['limit']);

$rows = array();
$ids = array();
$names = array();
$scores = array();
$dates = array();
$id = 0;

//Set limits
$abovelimit = floor($limit / 2);

//Find id of players best score
$stmt = $conn->prepare("SELECT * FROM leaderboard WHERE name = ? AND score = ? ORDER BY score DESC");
$stmt->bind_param("si", $name, $bestscore);
$stmt->execute();
$result = $stmt->get_result();
if ($result->num_rows > 0) {
    // output data of each row
    $rows = [];
    $position = 1;
    while($r = mysqli_fetch_assoc($result)) {
        $rows[] = $r;
	  $rows[$position -1][position] = $position;
	  $position++;
    }
  $id = $rows[0][id];
}
$stmt->close();

//Get all scores descending order
$sql = "SELECT id, name, score, date FROM leaderboard ORDER BY score DESC";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
    // output data of each rows
    $rows = [];
    while($r = mysqli_fetch_assoc($result)) {
	  $rows[] = $r;
        $ids[] = $r[id];
	  $names[] = $r[name];
	  $scores = $r[score];
	  $dates = $r[date];
    }
} else {
  echo "0";
}

//Get key of players best score
$key = array_search($id, $ids);


//Setup output array
$outputArray = array();
if ($key < $abovelimit) {
$startpoint = 0;
} else {
$startpoint = $key - $abovelimit;
}

$endpoint = $startpoint + ($limit - 1);
$index = 0;

for($i = $startpoint; $i <= $endpoint; $i++){
	if (!is_null($rows[$i])){
		$outputArray[$index] = $rows[$i];
		$outputArray[$index][position] = $i + 1;
	}
	$index++;
}

echo json_encode($outputArray);
$conn->close();
?>