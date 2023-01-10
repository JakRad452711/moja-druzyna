using moja_druzyna.Lib.PeselModule;
using Xunit;

namespace moja_druzyna_tests.Lib.PeselModule
{
    public class PeselTests
    {
        [Fact]
        public void GetDay_ShouldReturnProperDay()
        {
            int expected1 = 3;
            int expected2 = 12;
            int expected3 = 20;

            int actual1 = new Pesel("00410346279").GetDay();
            int actual2 = new Pesel("00471215628").GetDay();
            int actual3 = new Pesel("00522046348").GetDay();

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
        }

        [Fact]
        public void GetMonth_ShouldReturnProperMonth()
        {
            int expected1 = 1;
            int expected2 = 7;
            int expected3 = 12;

            int actual1 = new Pesel("00410346279").GetMonth();
            int actual2 = new Pesel("00471215628").GetMonth();
            int actual3 = new Pesel("00522046348").GetMonth();

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
        }

        [Fact]
        public void GetYear_ShouldReturnProperYear()
        {
            int expected1 = 1981;
            int expected2 = 2000;
            int expected3 = 2022;
            int expected4 = 2096;

            int actual1 = new Pesel("81122064233").GetYear();
            int actual2 = new Pesel("00322095261").GetYear();
            int actual3 = new Pesel("22322093766").GetYear();
            int actual4 = new Pesel("96322032935").GetYear();

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
            Assert.Equal(expected4, actual4);
        }

#warning should add some failing tests
        [Fact]
        public void IsValid_ShouldReturnTrueIfPeselIsValid()
        {
            bool expected1 = true;
            bool expected2 = true;
            bool expected3 = true;
            bool expected4 = true;

            bool actual1 = new Pesel("81122064233").IsValid();
            bool actual2 = new Pesel("00322095261").IsValid();
            bool actual3 = new Pesel("22322093766").IsValid();
            bool actual4 = new Pesel("96322032935").IsValid();

            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
            Assert.Equal(expected4, actual4);
        }
    }
}
