import { useState } from "react"
import { LoginDialog } from "@/components/LoginDialog"
import { Button } from "@/components/ui/button"
import "./index.css"

export default function App() {
  const [open, setOpen] = useState(true)

  return (
    <div className="flex min-h-svh w-full items-center justify-center bg-muted p-6 md:p-10">
      {!open && (
        <Button onClick={() => setOpen(true)}>Open Login Dialog</Button>
      )}
      <LoginDialog open={open} onOpenChange={setOpen} />
    </div>
  )
}
