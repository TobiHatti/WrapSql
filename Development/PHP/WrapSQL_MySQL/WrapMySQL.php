<?php

$startTime = new DateTime();

$i = 0;
while($i < 10000)
{
// Execute Query

// Connect to DB
$pdo = new PDO('mysql:host=localhost;dbname=endev', 'root', '', array( PDO::ATTR_PERSISTENT => true));


// Connect to DB with persistant connection (better performance for websites)
//$pdoPersistent = new PDO('mysql:host=localhost;dbname=endev', 'root', '',, array( PDO::ATTR_PERSISTENT => true));


// Get Result
$result = $pdo->query("Select * FROM projects");

// Cycle though rows
foreach($result as $row)
{
    echo $i.' '.$row['ProjectName']."\n";
}

$i++;

// Close connection
$pdo = null;
}

$endTime = new DateTime();

var_dump($endTime->diff($startTime, true));