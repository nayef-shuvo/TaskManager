import { useState } from "react";

function LoginPage() {
  const [formData, setFormData] = useState({ username: "", password: "" });

  const login = async () => {
    console.log(formData);
    let url = "http://localhost:5076/api/login";
    await fetch(url, { method: "POST" });
  };
  const handleChange = (event) => {
  };

  return (
    <div style={{ textAlign: 'center', paddingTop: '25%' }}>
      <h1>Login</h1>
      <div>
        <form method="POST" action={login}>
          <label>
            Username:
            <input style={{
              margin: '10px',
              height: '30px',
              width: '200px',
            }}
              type="text"
              value={formData.username}
              onChange={handleChange}
            />
          </label>
          <br />
          <label>
            Password:
            <input
              style={{
                margin: '10px',
                height: '30px',
                width: '200px',
              }}
              type="password"
              value={formData.password}
              onChange={handleChange}
            />
          </label>
          <br />
          <button type="submit" style={{
            color: 'black',
            height: '30px',
            width: '100px',
            textAlign: 'center',
            fontSize: '16px',
            marginTop: '10px',
            marginLeft: '70px',
            cursor: 'pointer'
          }}>Login</button>
        </form>
      </div>
    </div>
  );
}

export default LoginPage;
