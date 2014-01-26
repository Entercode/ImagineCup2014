CREATE FUNCTION [dbo].[StreetPassRating]
(
	@Time datetime2(2),
	@Parameter float
)
RETURNS float
AS
BEGIN
	DECLARE @second int;
	DECLARE @result float
	DECLARE @buf float

	SET @second = DATEDIFF(SECOND, @Time, DATEADD(HOUR, 9, GETDATE()));/*UTF->Tokyo*/

	IF  @second < 0 OR 100000 < @second OR @Parameter < 0 OR 100000 < @Parameter
		SET @result = -1.0;
	ELSE
	BEGIN
		SET @buf = @second * 10.0 / @Parameter;
		SET @result = @buf * @buf * @buf / 10.0;

		IF @result > 100.0
			SET @result = 100.0;
	END

	RETURN @result;
END