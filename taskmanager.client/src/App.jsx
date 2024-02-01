import { useState, useEffect } from 'react';
import LoginPage from './components/LoginPage';

function App() {
  const fetchData = async () => {
    try {
      let baseUrl = "http://localhost:5076/api";
      let response = await fetch(`${baseUrl}/users`);
      let data = await response.json();
      setUserData(data);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  }

  const [userData, setUserData] = useState(null);

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      {/* <h1>hello world</h1>
      {userData && (
        <div>
          {userData.map(user => (
            <div key={user.id}>{user.email}</div>
          ))}
        </div>
      )} */}
      <LoginPage></LoginPage>
    </>
  );
}

export default App;
