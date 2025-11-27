<?php
header("Access-Control-Allow-Origin: *");
$servername = "obscured";
$username = "obscured";
$password = "obscured";
$dbname = "obscured";
$secretKey = "obscured";

// Sanitize and validate inputs
$score = intval($_GET['score']);
$name = trim($_GET['name']);
$hash = $_GET['hash'];
$realHash = hash('sha256', $name . $score . $secretKey);

if($realHash == $hash) 
{ 
    // Create connection
    $conn = new mysqli($servername, $username, $password, $dbname);
    // Check connection
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    // Use prepared statements to prevent SQL injection
    if($name == "")
    {
        $stmt = $conn->prepare("INSERT INTO leaderboard (score) VALUES (?)");
        $stmt->bind_param("i", $score);
    } else {
        $stmt = $conn->prepare("INSERT INTO leaderboard (name, score) VALUES (?, ?)");
        $stmt->bind_param("si", $name, $score);
    }

    if ($stmt->execute()) {
        echo "New record created successfully";
    } else {
        echo "Error: " . $stmt->error;
    }

    $stmt->close();

$conn->close();
}
?>