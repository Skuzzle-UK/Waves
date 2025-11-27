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

$limit = intval($_GET['limit']); // Sanitize as integer

$stmt = $conn->prepare("SELECT name, score, date FROM leaderboard ORDER BY score DESC LIMIT ?");
$stmt->bind_param("i", $limit);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows > 0) {
    // output data of each row
    $rows = array();
    $position = 1;
    while($r = mysqli_fetch_assoc($result)) {
        $rows[] = $r;
	  $rows[$position - 1][position] = $position;
	  $position++;
    }
  echo json_encode($rows);
} else {
  echo "0";
}

$stmt->close();
$conn->close();
?>