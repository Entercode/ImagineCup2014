CREATE FUNCTION [dbo].[StreetPassRating]
(
	@Time DATETIME2(2),
	@Parameter FLOAT
)
RETURNS float
AS
BEGIN
	DECLARE @second INT;
	DECLARE @result FLOAT;
	DECLARE @x_first FLOAT;
	DECLARE @x_second FLOAT;

	SET @second = DATEDIFF(SECOND, @Time, DATEADD(HOUR, 9, GETDATE()));/*UTF->Tokyo*/

	IF  @second < 0 OR 100000 < @second OR @Parameter < 0 OR 100000 < @Parameter
		SET @result = -1.0;
	ELSE
	BEGIN
		SET @x_first = -1000.0 * @second / @Parameter + 200.0; 
		SET @x_second = @second * 10.0 / @Parameter;
		SET @x_second = @x_second * @x_second * @x_second / 10.0;

		IF @x_first > @x_second
			SET @result = @x_first;
		ELSE
			SET @result = @x_second

		IF @result > 100.0
			SET @result = 100.0;
	END

	RETURN @result;
END