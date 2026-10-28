using Fantasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Share
{
    public class RSAEncryptComponent :Entity
    {
        /// <summary>
        /// RSA私钥
        /// </summary>
        public string privateKey = "<RSAKeyValue><Modulus>1q/YySe0/py53tPVtqB9Ui/w/J6FptUfz+eEPIslD1aZsdjvZEXiDFlcorvZ10IAWvNKN1Uqhw53jALElXQsgf46Hkp2cnjk9lMCUkDOJUalxxveI8V4hLwJCNoATEH1j+Gol2Qc1gh+wGWm/SKPJIv64i4jbenyztQNj+5eQVE=</Modulus><Exponent>AQAB</Exponent><P>79S1rooW3tAzX76S1u2RMHmW7jsyuFN4Ryk2JEo75L3WRBczpmNawA+RuuLJuBPrrUGgnwNgSd6k5MpFN/3Izw==</P><Q>5Skr+fzIrZ4ivpHcgnOznRLAWl5GWHjITyrQ81obPiRghTJNbtf7QT3LXZ0VZWCOVGKnfRAQGGiu+gtuxJmb3w==</Q><DP>SiqioL5NgkwqhwJdUKAZFT+UwEmITt5pRMBm4OQgD4XxfA8cxujUyK5H5954eTeX23V/IY2eulli7unqa/1Mmw==</DP><DQ>zkg7j/Le6np5KDbA2qUwP+qLQE2w4R8IoUsXgJfSAiRYxk27qlW9lML9d61Zf0JlnMyZnFiDRJWnPOphsZPc3Q==</DQ><InverseQ>eS7yIrOIIEqCHPQfzP5BnqK0LWCsjVNp8sh/CI/9q2NKwWjhZ7Lcu9E/B9RgGV8feeJdbAh84ZAgy55cQsSRPQ==</InverseQ><D>U/WFVHXA5OoqDqjNpZRyLlgtaL4eCrbCsC4cmwjXv33gsZrjVFW4J5gzEDli4jofpXIMDkOyVAsmWY0jkEdUt/JoalXyOYVchx7UiqZy//6cIg1/hzsNO8F7XmrCR8SVbnyHg1PqAIXPnMQpjiX03ckoQSzbMA2E1Jtd8xhE6zk=</D></RSAKeyValue>";
        /// <summary>
        /// RSA公钥
        /// </summary>
        public string publicKey = "<RSAKeyValue><Modulus>1q/YySe0/py53tPVtqB9Ui/w/J6FptUfz+eEPIslD1aZsdjvZEXiDFlcorvZ10IAWvNKN1Uqhw53jALElXQsgf46Hkp2cnjk9lMCUkDOJUalxxveI8V4hLwJCNoATEH1j+Gol2Qc1gh+wGWm/SKPJIv64i4jbenyztQNj+5eQVE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    }
}
