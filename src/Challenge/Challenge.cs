namespace Challenge;
// CREATE TABLE IF NOT EXISTS ` + "`challenge`" + ` (
//     id INT unique AUTO_INCREMENT,
//     owner_id INT NOT NULL,
//     title VARCHAR(50) NOT NULL,
//     description VARCHAR(255) NOT NULL,
//     content LONGTEXT NOT NULL,
//     created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
//     updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
//     PRIMARY KEY (id),
//     FOREIGN KEY (owner_id) REFERENCES user(id),
//     UNIQUE INDEX (id)
// );

class Challenge {
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
