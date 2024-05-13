using System.ComponentModel;

namespace Lobby;
// CREATE TABLE IF NOT EXISTS lobby (
//     id INT AUTO_INCREMENT,
//     uuid VARCHAR(255) NOT NULL,
//     challenge_id INT NOT NULL,
//     owner_id INT NOT NULL,
//     ended BOOLEAN NOT NULL DEFAULT FALSE,

//     mode VARCHAR(50) NOT NULL,
//     max_players INT NOT NULL,
//     game_duration INT NOT NULL,
//     allowed_languages VARCHAR(255) NOT NULL,

//     created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
//     updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

//     PRIMARY KEY (id),
//     FOREIGN KEY (challenge_id) REFERENCES challenge(id),
//     FOREIGN KEY (owner_id) REFERENCES user(id),
//     UNIQUE INDEX (id),
//     UNIQUE INDEX (uuid)
// );

public class Entity {
    public int Id { get; set; }
    public required string Uuid { get; set; }
    public required Challenge.Entity Challenge { get; set; }
    public int OwnerId { get; set; }
    public bool Ended { get; set; }
    public required string Mode { get; set; }
    public required int MaxPlayers { get; set; }
    public required int GameDuration { get; set; }
    public required string AllowedLanguages { get; set; }
    [DefaultValue("CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    [DefaultValue("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// CREATE TABLE IF NOT EXISTS lobby_user (
//     id INT AUTO_INCREMENT,
//     lobby_id INT NOT NULL,
//     user_id INT NOT NULL,

//     code TEXT,
//     language VARCHAR(50),
//     tests_passed INT NOT NULL DEFAULT 0,
//     show_code BOOLEAN NOT NULL DEFAULT FALSE,
//     match_rank INT,
//     submitted_at TIMESTAMP,

//     created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
//     updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

//     PRIMARY KEY (id),
//     FOREIGN KEY (lobby_id) REFERENCES lobby(id),
//     FOREIGN KEY (user_id) REFERENCES user(id),
//     UNIQUE INDEX (id),
//     UNIQUE INDEX (lobby_id, user_id)
// );

public class UserEntity {
    public int Id { get; set; }
    public required int LobbyId { get; set; }
    public required User.Entity User { get; set; }
    public required string Code { get; set; }
    public required string Language { get; set; }
    public int TestsPassed { get; set; }
    public bool ShowCode { get; set; }
    public int? MatchRank { get; set; }
    public DateTime SubmittedAt { get; set; }
    [DefaultValue("CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; init; }
    [DefaultValue("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
}


// CREATE TABLE IF NOT EXISTS mode (
//     id INT unique AUTO_INCREMENT,
//     name VARCHAR(50) NOT NULL,
//     description VARCHAR(255) NOT NULL,

//     PRIMARY KEY (id),
//     UNIQUE INDEX (id)
// );

// INSERT IGNORE INTO mode
// 	(id, name, description) VALUES	
// 	(1, 'speed', 'The shortest time wins.'),
// 	(2, 'size', 'The shortest code wins.'),
// 	(3, 'efficiency', 'The most efficient code wins.'),
// 	(4, 'memory', 'The most memory efficient code wins.'),
// 	(5, 'readability', 'The most readable code wins.'),
// 	(6, 'style', 'The most stylish code wins.');

public class ModeEntity {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
