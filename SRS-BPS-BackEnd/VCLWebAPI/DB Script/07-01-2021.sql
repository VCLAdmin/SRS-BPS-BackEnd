
START TRANSACTION;

use VCLDesignDB;

DROP PROCEDURE IF EXISTS AddExternalClient;

DELIMITER //
CREATE PROCEDURE AddExternalClient()
BEGIN
	START TRANSACTION;
	SET @usr = 0;
    SET @ExistingDataCount = 0;
    
    SET @ExistingDataCount = (SELECT  Count(ClientId) FROM  ExternalClient WHERE ClientName = 'Rhino Queueing Server');
    SET @usr = (SELECT  UserId FROM  User WHERE Email = 'Administrator@vcldesign.com');
    SELECT @usr, @ExistingDataCount;
    
    IF @ExistingDataCount = 0 THEN
		INSERT INTO ExternalClient (`ClientExternalId`, `ClientName`, `ClientSecret`, `CreatedBy`,`CreatedOn`) 
		VALUES ('bee2c7a8-74dd-45ed-a8e9-95474b10eccb', 'Rhino Queueing Server', 'URjS-bOaulc21Bkj3TM_cRKMg622FSdCUq9xs2Icp-T26OowxIw4N-7SfBbmlH-O3UaLIfP8OsgQNAaE', @usr, NOW());
	END IF;
    COMMIT;
END //
DELIMITER ;

CALL AddExternalClient();
DROP PROCEDURE IF EXISTS AddExternalClient;

COMMIT;